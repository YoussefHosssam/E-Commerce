namespace E_Commerce.Domain.Common.Errors;

public static class RefundErrors
{
    public static readonly Error AmountInvalid = new(ErrorCodes.Refund.AmountInvalid, "Refund amount is invalid.", ErrorType.Validation);
    public static readonly Error PaymentIdRequired = new(ErrorCodes.Refund.PaymentIdRequired, "Payment ID is required.", ErrorType.Validation);
    public static readonly Error ProviderRefundIdRequired = new(ErrorCodes.Refund.ProviderRefundIdRequired, "Provider refund ID is required.", ErrorType.Validation);
    public static readonly Error StatusFinal = new(ErrorCodes.Refund.StatusFinal, "Refund status is final and cannot be changed.", ErrorType.Conflict);
}
