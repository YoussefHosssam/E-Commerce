using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;

public class TwoFactorLoginChallenge : BaseEntity
{
    public Guid UserId { get; private set; }
    public string ChallengeId { get; private set; } = default!;

    public int AttemptCount { get; private set; }
    public int MaxAttempts { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }

    private TwoFactorLoginChallenge() { }

    private TwoFactorLoginChallenge(
        Guid userId,
        string challengeId,
        DateTimeOffset expiresAt,
        int maxAttempts)
    {
        UserId = userId;
        ChallengeId = challengeId;
        ExpiresAt = expiresAt;
        MaxAttempts = maxAttempts;
    }

    public static TwoFactorLoginChallenge Create(
        Guid userId,
        string challengeId,
        DateTimeOffset expiresAt,
        int maxAttempts = 5)
    {
        return new TwoFactorLoginChallenge(userId, challengeId, expiresAt, maxAttempts);
    }

    public void MarkVerified(DateTimeOffset now)
    {
        if (IsExpired(now))
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.ChallengeExpired);

        VerifiedAt = now;
    }

    public void IncrementAttempts(DateTimeOffset now)
    {
        if (IsExpired(now))
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.ChallengeExpired);

        AttemptCount++;

        if (AttemptCount >= MaxAttempts)
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.ChallengeMaxAttempts);
    }

    public bool IsExpired(DateTimeOffset now)
        => now > ExpiresAt;

    public bool IsVerified => VerifiedAt is not null;
}
