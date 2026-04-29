namespace E_Commerce.Domain.Common.Errors;

public static class RefundErrors
{
    public static readonly Error AmountInvalid = new(ErrorCodes.Domain.Refund.AmountInvalid, "Refund amount is invalid.", ErrorType.Validation);
    public static readonly Error PaymentIdRequired = new(ErrorCodes.Domain.Refund.PaymentIdRequired, "Payment ID is required.", ErrorType.Validation);
    public static readonly Error ProviderRefundIdRequired = new(ErrorCodes.Domain.Refund.ProviderRefundIdRequired, "Provider refund ID is required.", ErrorType.Validation);
    public static readonly Error StatusFinal = new(ErrorCodes.Domain.Refund.StatusFinal, "Refund status is final and cannot be changed.", ErrorType.Conflict);
}
