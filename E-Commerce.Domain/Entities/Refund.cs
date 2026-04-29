using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;


namespace E_Commerce.Domain.Entities;
public class Refund : BaseEntity
{
    public Guid PaymentId { get; private set; }
    public string? ProviderRefundId { get; private set; }
    public RefundStatus Status { get; private set; } = RefundStatus.Requested;
    public decimal Amount { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    private Refund(){    }
    private Refund (Guid paymentId , decimal amount , string? reason)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
    }
    public static Refund Create( Guid paymentId, decimal amount, string? reason)
    {
        if (paymentId == Guid.Empty) throw new DomainValidationException(ErrorCodes.Domain.Refund.PaymentIdRequired);
        if (amount <= 0) throw new DomainValidationException(ErrorCodes.Domain.Refund.AmountInvalid);
        return new Refund(paymentId, amount, reason);
    }
    public void SetProviderRefundId(string providerRefundId)
    {
        if (string.IsNullOrEmpty(providerRefundId)) throw new DomainValidationException(ErrorCodes.Domain.Refund.ProviderRefundIdRequired);
        ProviderRefundId = providerRefundId;
        Touch(DateTimeOffset.UtcNow);
    }
    private void Touch(DateTimeOffset now)
    {
        UpdatedAt = now;
    }
    private void EnsureNotFinal()
    {
        if (Status is RefundStatus.Failed or RefundStatus.Succeeded)
        {
            throw new DomainValidationException(ErrorCodes.Domain.Refund.StatusFinal);
        }
    }
    public void MarkAsFailed()
    {
        EnsureNotFinal();
        Status = RefundStatus.Failed;
        Touch(DateTimeOffset.UtcNow);
    }
    public void MarkAsSucceeded()
    {
        EnsureNotFinal();
        Status = RefundStatus.Succeeded;
        Touch(DateTimeOffset.UtcNow);
    }
}


