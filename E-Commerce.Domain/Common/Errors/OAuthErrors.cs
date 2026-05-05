
namespace E_Commerce.Domain.Common.Errors;

public static class OAuthErrors
{
    public static readonly Error AlreadyLinked = new(ErrorCodes.OAuth.AlreadyLinked, "OAuth account is already linked.", ErrorType.Conflict);
    public static readonly Error Duplicate = new(ErrorCodes.OAuth.Duplicate, "Duplicate OAuth link.", ErrorType.Conflict);
    public static readonly Error EmailTooLong = new(ErrorCodes.OAuth.EmailTooLong, "OAuth email is too long.", ErrorType.Validation);
    public static readonly Error NotLinked = new(ErrorCodes.OAuth.NotLinked, "OAuth account is not linked.", ErrorType.NotFound);
    public static readonly Error NowRequired = new(ErrorCodes.OAuth.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error ProviderInvalid = new(ErrorCodes.OAuth.ProviderInvalid, "OAuth provider is invalid.", ErrorType.Validation);
    public static readonly Error ProviderRequired = new(ErrorCodes.OAuth.ProviderRequired, "OAuth provider is required.", ErrorType.Validation);
    public static readonly Error ProviderTooLong = new(ErrorCodes.OAuth.ProviderTooLong, "OAuth provider is too long.", ErrorType.Validation);
    public static readonly Error ProviderUserIdRequired = new(ErrorCodes.OAuth.ProviderUserIdRequired, "OAuth provider user ID is required.", ErrorType.Validation);
    public static readonly Error ProviderUserIdTooLong = new(ErrorCodes.OAuth.ProviderUserIdTooLong, "OAuth provider user ID is too long.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.OAuth.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
