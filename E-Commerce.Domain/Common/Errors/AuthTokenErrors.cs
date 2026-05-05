namespace E_Commerce.Domain.Common.Errors;

public static class AuthTokenErrors
{
    public static readonly Error ExpiresAtRequired = new(ErrorCodes.AuthToken.ExpiresAtRequired, "Token expiration date is required.", ErrorType.Validation);
    public static readonly Error TokenHashRequired = new(ErrorCodes.AuthToken.TokenHashRequired, "Token hash is required.", ErrorType.Validation);
    public static readonly Error TokenTypeRequired = new(ErrorCodes.AuthToken.TokenTypeRequired, "Token type is required.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.AuthToken.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
