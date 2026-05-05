namespace E_Commerce.Domain.Common.Errors;

public static class EmailMessageErrors
{
    public static readonly Error BodyRequired = new(ErrorCodes.EmailMessage.BodyRequired, "Email body is required.", ErrorType.Validation);
    public static readonly Error BodyTooLong = new(ErrorCodes.EmailMessage.BodyTooLong, "Email body is too long.", ErrorType.Validation);
    public static readonly Error ErrorRequired = new(ErrorCodes.EmailMessage.ErrorRequired, "Email error is required.", ErrorType.Validation);
    public static readonly Error ErrorTooLong = new(ErrorCodes.EmailMessage.ErrorTooLong, "Email error is too long.", ErrorType.Validation);
    public static readonly Error ProviderRequired = new(ErrorCodes.EmailMessage.ProviderRequired, "Email provider is required.", ErrorType.Validation);
    public static readonly Error ProviderTooLong = new(ErrorCodes.EmailMessage.ProviderTooLong, "Email provider is too long.", ErrorType.Validation);
    public static readonly Error SubjectRequired = new(ErrorCodes.EmailMessage.SubjectRequired, "Email subject is required.", ErrorType.Validation);
    public static readonly Error SubjectTooLong = new(ErrorCodes.EmailMessage.SubjectTooLong, "Email subject is too long.", ErrorType.Validation);
    public static readonly Error TypeInvalid = new(ErrorCodes.EmailMessage.TypeInvalid, "Email message type is invalid.", ErrorType.Validation);
}
