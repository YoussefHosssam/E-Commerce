namespace E_Commerce.Domain.Common.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new(ErrorCodes.Auth.InvalidCredentials, "Invalid email or password.", ErrorType.Unauthorized);
    public static readonly Error InvalidToken = new(ErrorCodes.Auth.InvalidToken, "Invalid authentication token.", ErrorType.Unauthorized);
    public static readonly Error ExpiredToken = new(ErrorCodes.Auth.ExpiredToken, "Authentication token has expired.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorRequired = new(ErrorCodes.Auth.TwoFactorRequired, "Two-factor verification is required.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorInvalid = new(ErrorCodes.Auth.TwoFactorInvalid, "Invalid two-factor authentication code.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorSetupRequired = new(ErrorCodes.Auth.TwoFactorSetupRequired, "Two-factor authentication must be set up first.", ErrorType.Conflict);
    public static readonly Error InvalidRequest = new(ErrorCodes.Auth.InvalidRequest, "The authentication request is invalid.", ErrorType.Validation);
    public static readonly Error PasswordInvalid = new(ErrorCodes.Auth.PasswordInvalid, "The supplied password is invalid.", ErrorType.Validation);
    public static readonly Error PasswordRequired = new(ErrorCodes.Auth.PasswordRequired, "Password is required.", ErrorType.Validation);
    public static readonly Error PasswordTooShort = new(ErrorCodes.Auth.PasswordTooShort, "Password must be at least 8 characters.", ErrorType.Validation);
    public static readonly Error PasswordTooLong = new(ErrorCodes.Auth.PasswordTooLong, "Password must not exceed 128 characters.", ErrorType.Validation);
    public static readonly Error PasswordUppercaseMissing = new(ErrorCodes.Auth.PasswordUppercaseMissing, "Password must contain at least one uppercase letter.", ErrorType.Validation);
    public static readonly Error PasswordLowercaseMissing = new(ErrorCodes.Auth.PasswordLowercaseMissing, "Password must contain at least one lowercase letter.", ErrorType.Validation);
    public static readonly Error PasswordDigitMissing = new(ErrorCodes.Auth.PasswordDigitMissing, "Password must contain at least one number.", ErrorType.Validation);
    public static readonly Error PasswordSpecialCharacterMissing = new(ErrorCodes.Auth.PasswordSpecialCharacterMissing, "Password must contain at least one special character.", ErrorType.Validation);
    public static readonly Error PasswordWhitespaceInvalid = new(ErrorCodes.Auth.PasswordWhitespaceInvalid, "Password must not contain spaces.", ErrorType.Validation);
    public static readonly Error EmailRequired = new(ErrorCodes.Auth.EmailRequired, "Email is required.", ErrorType.Validation);
    public static readonly Error EmailInvalid = new(ErrorCodes.Auth.EmailInvalid, "Email format is invalid.", ErrorType.Validation);
    public static readonly Error EmailTooLong = new(ErrorCodes.Auth.EmailTooLong, "Email must not exceed 256 characters.", ErrorType.Validation);
    public static readonly Error TokenRequired = new(ErrorCodes.Auth.TokenRequired, "Token can't be empty.", ErrorType.Validation);
}
