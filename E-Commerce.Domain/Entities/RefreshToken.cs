using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;
using System.Net;

namespace E_Commerce.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!; // EF navigation

    public TokenHash TokenHash { get; private set; } = default!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public TokenHash? ReplacedByTokenHash { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }

    public bool IsRevoked => RevokedAt is not null;
    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    private RefreshToken() { } // EF Core

    private RefreshToken(
        Guid userId,
        TokenHash tokenHash,
        DateTimeOffset expiresAt,
        string? deviceInfo,
        string? ipAddress)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        DeviceInfo = deviceInfo;
        IpAddress = ipAddress;
    }

    public static RefreshToken Create(
        Guid userId,
        TokenHash tokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset now,
        string? deviceInfo = null,
        string? ipAddress = null)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(RefreshErrors.UserIdRequired);

        if (tokenHash.Equals(default(TokenHash)))
            throw new DomainValidationException(RefreshErrors.TokenHashRequired);

        if (now == default)
            throw new DomainValidationException(RefreshErrors.NowRequired);

        if (expiresAt <= now)
            throw new DomainValidationException(RefreshErrors.ExpiresAtInvalid);

        if (deviceInfo is not null)
        {
            deviceInfo = deviceInfo.Trim();
            if (deviceInfo.Length > 300)
                throw new DomainValidationException(RefreshErrors.DeviceInfoTooLong);
        }

        if (ipAddress is not null)
        {
            ipAddress = ipAddress.Trim();
            if (ipAddress.Length > 45) // IPv6 max string
                throw new DomainValidationException(RefreshErrors.IpTooLong);

            // optional light validation (accept IPv4/IPv6)
            if (!IPAddress.TryParse(ipAddress, out _))
                throw new DomainValidationException(RefreshErrors.IpInvalid);
        }

        return new RefreshToken(userId, tokenHash, expiresAt, deviceInfo, ipAddress);
    }

    public void Revoke(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(RefreshErrors.NowRequired);

        if (IsRevoked)
            return; // idempotent (?? throw ?? ???)

        RevokedAt = now;
    }

    /// <summary>
    /// Marks this token as revoked and sets the replacement token hash.
    /// </summary>
    public void Replace(TokenHash replacedByTokenHash, DateTimeOffset now)
    {
        if (replacedByTokenHash.Equals(default(TokenHash)))
            throw new DomainValidationException(RefreshErrors.ReplacedByRequired);

        if (replacedByTokenHash.Equals(TokenHash))
            throw new DomainValidationException(RefreshErrors.ReplacedBySame);

        ReplacedByTokenHash = replacedByTokenHash;
        Revoke(now);
    }
}
