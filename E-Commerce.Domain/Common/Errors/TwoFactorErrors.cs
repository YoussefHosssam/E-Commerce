namespace E_Commerce.Domain.Common.Errors;

public static class TwoFactorErrors
{
    public static readonly Error AlreadyDisabled = new(ErrorCodes.Domain.TwoFactor.AlreadyDisabled, "Two-factor authentication is already disabled.", ErrorType.Conflict);
    public static readonly Error AlreadyEnabled = new(ErrorCodes.Domain.TwoFactor.AlreadyEnabled, "Two-factor authentication is already enabled.", ErrorType.Conflict);
    public static readonly Error AlreadyVerified = new(ErrorCodes.Domain.TwoFactor.AlreadyVerified, "Two-factor authentication is already verified.", ErrorType.Conflict);
    public static readonly Error ChallengeExpired = new(ErrorCodes.Domain.TwoFactor.ChallengeExpired, "Two-factor challenge has expired.", ErrorType.Validation);
    public static readonly Error ChallengeMaxAttempts = new(ErrorCodes.Domain.TwoFactor.ChallengeMaxAttempts, "Two-factor challenge maximum attempts exceeded.", ErrorType.Validation);
    public static readonly Error NotEnabled = new(ErrorCodes.Domain.TwoFactor.NotEnabled, "Two-factor authentication is not enabled.", ErrorType.NotFound);
    public static readonly Error NotSetup = new(ErrorCodes.Domain.TwoFactor.NotSetup, "Two-factor authentication is not set up.", ErrorType.NotFound);
    public static readonly Error RecoveryInvalid = new(ErrorCodes.Domain.TwoFactor.RecoveryInvalid, "Recovery code is invalid.", ErrorType.Validation);
    public static readonly Error RecoveryUsed = new(ErrorCodes.Domain.TwoFactor.RecoveryUsed, "Recovery code has already been used.", ErrorType.Conflict);
    public static readonly Error SecretRequired = new(ErrorCodes.Domain.TwoFactor.SecretRequired, "Two-factor secret is required.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Domain.TwoFactor.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
