using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public class User : BaseEntity
{
    public EmailAddress Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string? Phone { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Customer;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public bool IsPasswordUpdated { get; set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsTwoFactorAuthEnabled { get; private set; } = false;
    public UserCredential Credential { get; private set; } = default!;

    private readonly List<UserOAuthAccount> _oauthAccounts = new();
    public IReadOnlyCollection<UserOAuthAccount> OAuthAccounts
        => _oauthAccounts.AsReadOnly();
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens
        => _refreshTokens.AsReadOnly();
    public UserTwoFactor? TwoFactorAuth { get; private set; }

    private readonly List<Cart> _carts = new();
    public IReadOnlyCollection<Cart> Carts
        => _carts.AsReadOnly();

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders
        => _orders.AsReadOnly();

    private readonly List<Favorite> _favorites = new();
    public IReadOnlyCollection<Favorite> Favorites
        => _favorites.AsReadOnly();

    private readonly List<StockAlert> _stockAlerts = new();
    public IReadOnlyCollection<StockAlert> StockAlerts
        => _stockAlerts.AsReadOnly();

    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications
        => _notifications.AsReadOnly();
    private readonly List<AuditLog> _auditLogsAsActor = new();
    public IReadOnlyCollection<AuditLog> AuditLogsAsActor
        => _auditLogsAsActor.AsReadOnly();
    private readonly List<TwoFactorRecoveryCode> _recoveryCodes = new();
    public IReadOnlyCollection<TwoFactorRecoveryCode> RecoveryCodes => _recoveryCodes.AsReadOnly();

    // EF Core
    private User() { }

    private User(EmailAddress email, string firstName, string lastName, string? phone, UserRole role = UserRole.Customer)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        FullName = BuildFullName(firstName, lastName);
        Phone = phone;
        Role = role;
        VerificationStatus = VerificationStatus.Pending;
        IsActive = true;
        UpdatedAt = null;
    }

    public static User Create(
        string email,
        PasswordHash password,
        string firstName,
        string lastName,
        string? phoneNumber,
        UserRole role = UserRole.Customer)
    {
        var emailAddress = EmailAddress.Create(email);

        firstName = NormalizeName(firstName, ErrorCodes.User.FirstNameRequired);
        lastName = NormalizeName(lastName, ErrorCodes.User.LastNameRequired);

        if (phoneNumber is not null)
        {
            phoneNumber = phoneNumber.Trim();
            if (phoneNumber.Length > 30)
                throw new DomainValidationException(ErrorCodes.User.PhoneInvalid);
        }

        // ?? ???? roles ????? ?? enum ??? ????? ??????? ?? ???? ???? undefined
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new DomainValidationException(ErrorCodes.User.RoleInvalid);
        User user = new User(emailAddress, firstName, lastName, phoneNumber, role);
        user.Credential = UserCredential.Create(user.Id , password);
        return user;
    }

    public void ChangeName(string firstName, string lastName)
    {
        firstName = NormalizeName(firstName, ErrorCodes.User.FirstNameRequired);
        lastName = NormalizeName(lastName, ErrorCodes.User.LastNameRequired);

        FirstName = firstName;
        LastName = lastName;
        FullName = BuildFullName(firstName, lastName);
        Touch();
    }

    public void ChangePhone(string? phoneNumber)
    {
        if (phoneNumber is not null)
        {
            phoneNumber = phoneNumber.Trim();
            if (phoneNumber.Length > 30)
                throw new DomainValidationException(ErrorCodes.User.PhoneInvalid);
        }

        Phone = phoneNumber;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
    public void Verify()
    {
        VerificationStatus = VerificationStatus.Verified;
    }
    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    private static string NormalizeName(string value, string code)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(code);

        value = value.Trim();

        if (value.Length > 100)
            throw new DomainValidationException(ErrorCodes.User.NameTooLong);

        return value;
    }

    private static string BuildFullName(string firstName, string lastName)
        => $"{firstName} {lastName}".Trim();
    public void SetupTwoFactor(string encryptedSecret, DateTimeOffset now)
    {
        // ensure TwoFa exists
        TwoFactorAuth ??= UserTwoFactor.Create(this.Id);

        TwoFactorAuth.Setup(encryptedSecret, now);
        Touch(now);
    }
    public void EnableTwoFactor(DateTimeOffset now)
    {
        IsTwoFactorAuthEnabled = true;
        Touch(now);
    }

    public void DisableTwoFactor(DateTimeOffset now)
    {
        if (TwoFactorAuth is null)
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.NotSetup);

        TwoFactorAuth.Disable(now);
        IsTwoFactorAuthEnabled = false;
        Touch(now);
    }

    public void GenerateRecoveryCodes(IEnumerable<string> hashedCodes, DateTimeOffset now)
    {
        if (!IsTwoFactorAuthEnabled)
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.NotEnabled);

        _recoveryCodes.Clear();

        foreach (var hash in hashedCodes)
        {
            _recoveryCodes.Add(TwoFactorRecoveryCode.Create(this.Id, hash));
        }

        Touch(now);
    }
    public void UseRecoveryCode(string providedCode, Func<string, string, bool> verifyHash, DateTimeOffset now)
    {
        var code = _recoveryCodes.FirstOrDefault(x =>
            !x.IsUsed && verifyHash(x.CodeHash, providedCode));

        if (code is null)
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.RecoveryInvalid);

        code.MarkAsUsed(now);
        Touch(now);
    }
    public UserOAuthAccount LinkOAuthAccount(
    string provider,
    string providerUserId,
    string? email,
    DateTimeOffset now)
    {
        // rule: ?? ???? ??? provider ?????
        var normalizedProvider = provider?.Trim().ToLowerInvariant();

        if (_oauthAccounts.Any(x => x.Provider == normalizedProvider))
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.AlreadyLinked);

        // rule: ?? ???? ??? providerUserId ??? ??? (???? ??? ??????)
        if (_oauthAccounts.Any(x => x.Provider == normalizedProvider && x.ProviderUserId == providerUserId.Trim()))
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.Duplicate);

        var account = UserOAuthAccount.Create(this.Id, provider, providerUserId, email, now);
        _oauthAccounts.Add(account);
        Touch(now);
        return account;
    }

    public void UnlinkOAuthAccount(string provider, DateTimeOffset now)
    {
        provider = (provider ?? "").Trim().ToLowerInvariant();

        var idx = _oauthAccounts.FindIndex(x => x.Provider == provider);
        if (idx < 0)
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.NotLinked);

        _oauthAccounts.RemoveAt(idx);
        Touch(now);
    }
    private void Touch()
        => UpdatedAt = DateTimeOffset.UtcNow;
    private void Touch(DateTimeOffset now) => UpdatedAt = now;

}

