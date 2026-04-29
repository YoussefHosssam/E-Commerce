using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities
{
    public class AuthToken : BaseEntity
    {
        public Guid UserId { get; private set; }

        public TokenHash TokenHash { get; private set; } = default!;
        public TokenType TokenType { get; private set; } = default!;
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? ConsumedAt { get; private set; }

        public User User { get; set; } = default!;
        private AuthToken(Guid userId ,TokenType tokenType , TokenHash tokenHash , DateTimeOffset expiresAt)
        {
            this.TokenHash = tokenHash;
            this.TokenType = tokenType;
            this.UserId = userId;
            this.ExpiresAt = expiresAt;
        }
        public static AuthToken Create(Guid userId, TokenType tokenType , TokenHash tokenHash, DateTimeOffset expiresAt)
        {
            if (userId == Guid.Empty)
                throw new DomainValidationException(ErrorCodes.Domain.AuthToken.UserIdRequired);
            if (string.IsNullOrEmpty(tokenHash.Value))
                throw new DomainValidationException(ErrorCodes.Domain.AuthToken.TokenHashRequired);
            if (expiresAt <= DateTimeOffset.UtcNow)
                throw new DomainValidationException(ErrorCodes.Domain.AuthToken.ExpiresAtRequired);
            if (!Enum.IsDefined(typeof(TokenType) , tokenType))
                throw new DomainValidationException(ErrorCodes.Domain.AuthToken.TokenTypeRequired);
            return new AuthToken(userId , tokenType, tokenHash, expiresAt);
        }
        public void Consume()
        {
            ConsumedAt = DateTimeOffset.UtcNow;
        }
        public bool IsTokenExpired(DateTimeOffset now)
        {
            return now > ExpiresAt;
        }
        public bool IsTokenConsumed()
        {
            return ConsumedAt.HasValue;
        }
    }
}


