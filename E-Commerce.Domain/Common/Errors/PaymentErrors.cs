namespace E_Commerce.Domain.Common.Errors;

public static class PaymentErrors
{
    public static readonly Error AmountInvalid = new(ErrorCodes.Payment.AmountInvalid, "Payment amount is invalid.", ErrorType.Validation);
    public static readonly Error CurrencyRequired = new(ErrorCodes.Payment.CurrencyRequired, "Payment currency is required.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Payment.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error OrderIdRequired = new(ErrorCodes.Payment.OrderIdRequired, "Order ID is required.", ErrorType.Validation);
    public static readonly Error ProviderRequired = new(ErrorCodes.Payment.ProviderRequired, "Payment provider is required.", ErrorType.Validation);
    public static readonly Error ProviderTooLong = new(ErrorCodes.Payment.ProviderTooLong, "Payment provider is too long.", ErrorType.Validation);
    public static readonly Error ProviderPaymentIdRequired = new(ErrorCodes.Payment.ProviderPaymentIdRequired, "Provider payment ID is required.", ErrorType.Validation);
    public static readonly Error ProviderPaymentIdTooLong = new(ErrorCodes.Payment.ProviderPaymentIdTooLong, "Provider payment ID is too long.", ErrorType.Validation);
    public static readonly Error RefundNotCaptured = new(ErrorCodes.Payment.RefundNotCaptured, "Payment cannot be refunded because it was not captured.", ErrorType.Conflict);
    public static readonly Error StatusFinal = new(ErrorCodes.Payment.StatusFinal, "Payment status is final and cannot be changed.", ErrorType.Conflict);
    public static readonly Error StatusInvalidTransition = new(ErrorCodes.Payment.StatusInvalidTransition, "Payment status transition is invalid.", ErrorType.Conflict);
}
