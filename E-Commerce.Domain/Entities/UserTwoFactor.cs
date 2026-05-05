using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class UserTwoFactor : BaseEntity
{
    public Guid UserId { get; private set; }
    public bool IsEnabled { get; private set; }
    public string TwoFactorAuthMethod { get; private set; } = "totp";
    public string? TotpSecretEncrypted { get; private set; }
    public bool IsVerified { get; private set; } = false;
    public DateTimeOffset? EnabledAt { get; private set; }
    public DateTimeOffset? DisabledAt { get; private set; }

    // EF Core navigation (???????)
    public User User { get; private set; } = default!;

    private UserTwoFactor() { } // EF

    private UserTwoFactor(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(TwoFactorErrors.UserIdRequired);

        UserId = userId;
        IsEnabled = false;
    }

    internal static UserTwoFactor Create(Guid userId) => new UserTwoFactor(userId);

    internal void Setup(string totpSecretEncrypted, DateTimeOffset now)
    {
        if (IsEnabled)
            throw new DomainValidationException(TwoFactorErrors.AlreadyEnabled);

        if (string.IsNullOrWhiteSpace(totpSecretEncrypted))
            throw new DomainValidationException(TwoFactorErrors.SecretRequired);
        IsEnabled = true;
        EnabledAt = now;
        IsVerified = false;
        TotpSecretEncrypted = totpSecretEncrypted.Trim();
        DisabledAt = null;
    }
    internal void Verify()
    {
        if (IsVerified || !IsEnabled)
            throw new DomainValidationException(TwoFactorErrors.AlreadyVerified);
        IsVerified = true;
        DisabledAt = null;
    }

    internal void Disable(DateTimeOffset now)
    {
        if (!IsEnabled)
            throw new DomainValidationException(TwoFactorErrors.AlreadyDisabled);

        IsEnabled = false;
        DisabledAt = now;

        // ????: ?? ???? ???? ??? ??????
        // ?????? ????? ????? ????:
        TotpSecretEncrypted = null;
    }
}

