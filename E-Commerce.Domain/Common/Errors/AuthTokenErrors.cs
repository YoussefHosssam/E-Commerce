namespace E_Commerce.Domain.Common.Errors;

public static class AuthTokenErrors
{
    public static readonly Error ExpiresAtRequired = new(ErrorCodes.Domain.AuthToken.ExpiresAtRequired, "Token expiration date is required.", ErrorType.Validation);
    public static readonly Error TokenHashRequired = new(ErrorCodes.Domain.AuthToken.TokenHashRequired, "Token hash is required.", ErrorType.Validation);
    public static readonly Error TokenTypeRequired = new(ErrorCodes.Domain.AuthToken.TokenTypeRequired, "Token type is required.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Domain.AuthToken.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
