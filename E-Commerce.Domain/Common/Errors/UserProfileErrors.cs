namespace E_Commerce.Domain.Common.Errors;

public static class UserProfileErrors
{
    public static readonly Error NotFound = new(ErrorCodes.UserProfile.NotFound, "User profile was not found.", ErrorType.NotFound);
    public static readonly Error UserIdRequired = new(ErrorCodes.UserProfile.UserIdRequired, "User id is required.", ErrorType.Validation);
    public static readonly Error FirstNameRequired = new(ErrorCodes.UserProfile.FirstNameRequired, "First name is required.", ErrorType.Validation);
    public static readonly Error LastNameRequired = new(ErrorCodes.UserProfile.LastNameRequired, "Last name is required.", ErrorType.Validation);
    public static readonly Error NameTooLong = new(ErrorCodes.UserProfile.NameTooLong, "Name must not exceed 100 characters.", ErrorType.Validation);
    public static readonly Error DisplayNameTooLong = new(ErrorCodes.UserProfile.DisplayNameTooLong, "Display name must not exceed 150 characters.", ErrorType.Validation);
    public static readonly Error DateOfBirthInvalid = new(ErrorCodes.UserProfile.DateOfBirthInvalid, "Date of birth cannot be in the future.", ErrorType.Validation);
    public static readonly Error AvatarUrlInvalid = new(ErrorCodes.UserProfile.AvatarUrlInvalid, "Avatar URL is invalid.", ErrorType.Validation);
    public static readonly Error GenderInvalid = new(ErrorCodes.UserProfile.GenderInvalid, "Gender is invalid.", ErrorType.Validation);
}
