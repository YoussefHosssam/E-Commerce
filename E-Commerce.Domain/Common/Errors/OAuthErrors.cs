namespace E_Commerce.Domain.Common.Errors;

public static class OAuthErrors
{
    public static readonly Error AlreadyLinked = new(ErrorCodes.Domain.OAuth.AlreadyLinked, "OAuth account is already linked.", ErrorType.Conflict);
    public static readonly Error Duplicate = new(ErrorCodes.Domain.OAuth.Duplicate, "Duplicate OAuth link.", ErrorType.Conflict);
    public static readonly Error EmailTooLong = new(ErrorCodes.Domain.OAuth.EmailTooLong, "OAuth email is too long.", ErrorType.Validation);
    public static readonly Error NotLinked = new(ErrorCodes.Domain.OAuth.NotLinked, "OAuth account is not linked.", ErrorType.NotFound);
    public static readonly Error NowRequired = new(ErrorCodes.Domain.OAuth.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error ProviderInvalid = new(ErrorCodes.Domain.OAuth.ProviderInvalid, "OAuth provider is invalid.", ErrorType.Validation);
    public static readonly Error ProviderRequired = new(ErrorCodes.Domain.OAuth.ProviderRequired, "OAuth provider is required.", ErrorType.Validation);
    public static readonly Error ProviderTooLong = new(ErrorCodes.Domain.OAuth.ProviderTooLong, "OAuth provider is too long.", ErrorType.Validation);
    public static readonly Error ProviderUserIdRequired = new(ErrorCodes.Domain.OAuth.ProviderUserIdRequired, "OAuth provider user ID is required.", ErrorType.Validation);
    public static readonly Error ProviderUserIdTooLong = new(ErrorCodes.Domain.OAuth.ProviderUserIdTooLong, "OAuth provider user ID is too long.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Domain.OAuth.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
