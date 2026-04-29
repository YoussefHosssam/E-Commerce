namespace E_Commerce.Domain.Common.Errors;

public static class UserCredentialErrors
{
    public static readonly Error PasswordHashInvalid = new(ErrorCodes.Domain.UserCredential.PasswordHashInvalid, "Password hash is invalid.", ErrorType.Validation);
    public static readonly Error PasswordHashRequired = new(ErrorCodes.Domain.UserCredential.PasswordHashRequired, "Password hash is required.", ErrorType.Validation);
    public static readonly Error UserIdInvalid = new(ErrorCodes.Domain.UserCredential.UserIdInvalid, "User ID is invalid.", ErrorType.Validation);
}
