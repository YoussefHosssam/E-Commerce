
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities
{
    public class UserCredential : BaseEntity
    {
        public Guid UserId { get; private set; }
        public PasswordHash PasswordHash { get; private set; } = default!;
        public DateTimeOffset? UpdatedAt { get; private set; }

        // EF Core
        private UserCredential() { }

        private UserCredential(Guid userId, PasswordHash passwordHash)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public static UserCredential Create(Guid userId, PasswordHash passwordHash)
        {
            if (userId == Guid.Empty)
                throw new DomainValidationException(UserCredentialErrors.UserIdInvalid);
            if (string.IsNullOrWhiteSpace(passwordHash.Value))
                throw new DomainValidationException(UserCredentialErrors.PasswordHashRequired);
            if (passwordHash.Value.Length < 20)
                throw new DomainValidationException(UserCredentialErrors.PasswordHashInvalid);

            return new UserCredential(userId, passwordHash);
        }

        public void SetPassword(PasswordHash password)
        {
            PasswordHash = password;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
