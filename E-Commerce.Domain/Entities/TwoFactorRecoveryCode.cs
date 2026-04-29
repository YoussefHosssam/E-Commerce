using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;

public class TwoFactorRecoveryCode : BaseEntity
{
    public Guid UserId { get; private set; }
    public string CodeHash { get; private set; } = default!;
    public DateTimeOffset? UsedAt { get; private set; }

    private TwoFactorRecoveryCode() { }

    private TwoFactorRecoveryCode(Guid userId, string codeHash)
    {
        UserId = userId;
        CodeHash = codeHash;
    }

    public static TwoFactorRecoveryCode Create(Guid userId, string codeHash)
    {
        return new TwoFactorRecoveryCode(userId, codeHash);
    }

    public void MarkAsUsed(DateTimeOffset now)
    {
        if (UsedAt is not null)
            throw new DomainValidationException(ErrorCodes.Domain.TwoFactor.RecoveryUsed);

        UsedAt = now;
    }

    public bool IsUsed => UsedAt is not null;
}

