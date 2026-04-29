using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class UserOAuthAccount : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!; // EF navigation

    public string Provider { get; private set; } = default!;         // "google"
    public string ProviderUserId { get; private set; } = default!;   // subject/id from provider
    public string? Email { get; private set; }
    public DateTimeOffset LinkedAt { get; private set; }

    private UserOAuthAccount() { } // EF Core

    private UserOAuthAccount(
        Guid userId,
        string provider,
        string providerUserId,
        string? email,
        DateTimeOffset linkedAt)
    {
        UserId = userId;
        Provider = provider;
        ProviderUserId = providerUserId;
        Email = email;
        LinkedAt = linkedAt;
    }

    public static UserOAuthAccount Create(
        Guid userId,
        string provider,
        string providerUserId,
        string? email,
        DateTimeOffset now)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.UserIdRequired);

        if (string.IsNullOrWhiteSpace(provider))
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.ProviderRequired);

        provider = provider.Trim().ToLowerInvariant();

        // ???????: ?? providers ???? ?????? ?????? ???:
        // if (provider is not "google" and not "apple" and not "facebook")
        //     throw new DomainValidationException(ErrorCodes.Domain.OAuth.ProviderInvalid);

        if (provider.Length > 30)
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.ProviderTooLong);

        if (string.IsNullOrWhiteSpace(providerUserId))
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.ProviderUserIdRequired);

        providerUserId = providerUserId.Trim();
        if (providerUserId.Length > 200)
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.ProviderUserIdTooLong);

        if (email is not null)
        {
            email = email.Trim();
            if (email.Length > 254)
                throw new DomainValidationException(ErrorCodes.Domain.OAuth.EmailTooLong);
        }

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.OAuth.NowRequired);

        return new UserOAuthAccount(userId, provider, providerUserId, email, now);
    }
}
