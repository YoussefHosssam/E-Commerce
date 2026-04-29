namespace E_Commerce.Domain.Common.Errors;

public static class ExternalErrors
{
    public static readonly Error ServiceUnavailable = new(ErrorCodes.External.ServiceUnavailable, "A required external service is temporarily unavailable.", ErrorType.External);
    public static readonly Error PaymentProviderUnavailable = new(ErrorCodes.External.PaymentProviderUnavailable, "The payment provider is temporarily unavailable.", ErrorType.External);
    public static readonly Error EmailProviderUnavailable = new(ErrorCodes.External.EmailProviderUnavailable, "The email provider is temporarily unavailable.", ErrorType.External);
}
