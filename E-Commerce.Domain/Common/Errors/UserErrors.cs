namespace E_Commerce.Domain.Common.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = new(ErrorCodes.User.NotFound, "User not found.", ErrorType.NotFound);
    public static readonly Error EmailAlreadyExists = new(ErrorCodes.User.EmailAlreadyExists, "An account with this email already exists.", ErrorType.Conflict);
    public static readonly Error EmailRequired = new(ErrorCodes.User.EmailRequired, "Email is required.", ErrorType.Validation);
    public static readonly Error EmailInvalid = new(ErrorCodes.User.EmailInvalid, "Email format is invalid.", ErrorType.Validation);
    public static readonly Error EmailTooLong = new(ErrorCodes.User.EmailTooLong, "Email must not exceed 256 characters.", ErrorType.Validation);
    public static readonly Error FirstNameRequired = new(ErrorCodes.User.FirstNameRequired, "First name is required.", ErrorType.Validation);
    public static readonly Error FirstNameTooShort = new(ErrorCodes.User.FirstNameTooShort, "First name must be at least 2 characters.", ErrorType.Validation);
    public static readonly Error FirstNameTooLong = new(ErrorCodes.User.FirstNameTooLong, "First name must not exceed 100 characters.", ErrorType.Validation);
    public static readonly Error LastNameRequired = new(ErrorCodes.User.LastNameRequired, "Last name is required.", ErrorType.Validation);
    public static readonly Error LastNameTooShort = new(ErrorCodes.User.LastNameTooShort, "Last name must be at least 2 characters.", ErrorType.Validation);
    public static readonly Error LastNameTooLong = new(ErrorCodes.User.LastNameTooLong, "Last name must not exceed 100 characters.", ErrorType.Validation);
    public static readonly Error PhoneRequired = new(ErrorCodes.User.PhoneRequired, "Phone number is required.", ErrorType.Validation);
    public static readonly Error PhoneTooLong = new(ErrorCodes.User.PhoneTooLong, "Phone number must not exceed 30 characters.", ErrorType.Validation);
    public static readonly Error PhoneInvalid = new(ErrorCodes.User.PhoneInvalid, "Phone number format is invalid.", ErrorType.Validation);
    public static readonly Error NameTooLong = new(ErrorCodes.User.NameTooLong, "Name must not exceed 100 characters.", ErrorType.Validation);
    public static readonly Error RoleInvalid = new(ErrorCodes.User.RoleInvalid, "User role is invalid.", ErrorType.Validation);
}
