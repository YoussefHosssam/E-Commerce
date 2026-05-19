using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;

namespace E_Commerce.Domain.Entities;

public sealed class UserProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public Gender? Gender { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private UserProfile() { }

    private UserProfile(Guid userId, string firstName, string lastName, string? displayName)
    {
        UserId = userId;
        FirstName = NormalizeRequired(firstName, UserProfileErrors.FirstNameRequired, 100);
        LastName = NormalizeRequired(lastName, UserProfileErrors.LastNameRequired, 100);
        DisplayName = NormalizeOptional(displayName, 150) ?? BuildDisplayName(FirstName, LastName);
    }

    public static UserProfile Create(Guid userId, string firstName, string lastName, string? displayName = null)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(UserProfileErrors.UserIdRequired);

        return new UserProfile(userId, firstName, lastName, displayName);
    }

    public void Update(
        string firstName,
        string lastName,
        string? displayName,
        Gender? gender,
        DateOnly? dateOfBirth,
        string? avatarUrl,
        DateTimeOffset now)
    {
        FirstName = NormalizeRequired(firstName, UserProfileErrors.FirstNameRequired, 100);
        LastName = NormalizeRequired(lastName, UserProfileErrors.LastNameRequired, 100);
        DisplayName = NormalizeOptional(displayName, 150) ?? BuildDisplayName(FirstName, LastName);

        if (gender.HasValue && !Enum.IsDefined(typeof(Gender), gender.Value))
            throw new DomainValidationException(UserProfileErrors.GenderInvalid);

        if (dateOfBirth.HasValue && dateOfBirth.Value > DateOnly.FromDateTime(now.UtcDateTime))
            throw new DomainValidationException(UserProfileErrors.DateOfBirthInvalid);

        AvatarUrl = NormalizeUrlOrNull(avatarUrl);
        Gender = gender;
        DateOfBirth = dateOfBirth;
        UpdatedAt = now;
    }

    private static string BuildDisplayName(string firstName, string lastName)
        => $"{firstName} {lastName}".Trim();

    private static string NormalizeRequired(string value, Error requiredError, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(requiredError);

        value = value.Trim();

        if (value.Length > maxLength)
            throw new DomainValidationException(UserProfileErrors.NameTooLong);

        return value;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (value is null)
            return null;

        value = value.Trim();
        if (value.Length == 0)
            return null;

        if (value.Length > maxLength)
            throw new DomainValidationException(UserProfileErrors.DisplayNameTooLong);

        return value;
    }

    private static string? NormalizeUrlOrNull(string? value)
    {
        if (value is null)
            return null;

        value = value.Trim();
        if (value.Length == 0)
            return null;

        if (value.Length > 1000 ||
            !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            uri.Scheme is not ("http" or "https"))
        {
            throw new DomainValidationException(UserProfileErrors.AvatarUrlInvalid);
        }

        return value;
    }
}
