using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public string Provider { get; private set; } = default!; // "paymob|..."
    public string? ProviderPaymentId { get; private set; }

    public PaymentStatus Status { get; private set; } = PaymentStatus.Initiated;

    public decimal Amount { get; private set; }
    public CurrencyCode Currency { get; private set; } = CurrencyCode.Create("EGP");

    public JsonText RawPayloadJson { get; private set; } = JsonText.Create("{}");
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<Refund> _refunds = new();
    public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();

    private Payment() { } // EF

    private Payment(
        Guid orderId,
        string provider,
        decimal amount,
        CurrencyCode currency,
        JsonText rawPayloadJson,
        DateTimeOffset now)
    {
        OrderId = orderId;
        Provider = provider;
        Amount = amount;
        Currency = currency;
        RawPayloadJson = rawPayloadJson;

        Status = PaymentStatus.Initiated;
        UpdatedAt = now;
    }

    public static Payment Create(
        Guid orderId,
        string provider,
        decimal amount,
        CurrencyCode currency,
        JsonText? rawPayloadJson,
        DateTimeOffset now)
    {
        if (orderId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.OrderIdRequired);

        if (string.IsNullOrWhiteSpace(provider))
            throw new DomainValidationException(ErrorCodes.Domain.Payment.ProviderRequired);

        provider = provider.Trim().ToLowerInvariant();
        if (provider.Length > 30)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.ProviderTooLong);

        if (amount <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.AmountInvalid);

        if (currency.Equals(default(CurrencyCode)))
            throw new DomainValidationException(ErrorCodes.Domain.Payment.CurrencyRequired);

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.NowRequired);

        rawPayloadJson ??= JsonText.Create("{}");

        return new Payment(orderId, provider, amount, currency, rawPayloadJson.Value, now);
    }

    // --------- Domain transitions ---------

    public void AttachProviderPaymentId(string providerPaymentId, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(providerPaymentId))
            throw new DomainValidationException(ErrorCodes.Domain.Payment.ProviderPaymentIdRequired);

        providerPaymentId = providerPaymentId.Trim();
        if (providerPaymentId.Length > 120)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.ProviderPaymentIdTooLong);

        ProviderPaymentId = providerPaymentId;
        Touch(now);
    }

    public void MarkAuthorized(DateTimeOffset now)
    {
        EnsureNotFinal();
        if (Status != PaymentStatus.Initiated)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.StatusInvalidTransition);

        Status = PaymentStatus.Authorized;
        Touch(now);
    }

    public void MarkCaptured(DateTimeOffset now)
    {
        EnsureNotFinal();
        if (Status is not (PaymentStatus.Initiated or PaymentStatus.Authorized))
            throw new DomainValidationException(ErrorCodes.Domain.Payment.StatusInvalidTransition);

        Status = PaymentStatus.Captured;
        Touch(now);
    }

    public void MarkFailed(DateTimeOffset now, JsonText? rawPayloadJson = null)
    {
        EnsureNotFinal();

        Status = PaymentStatus.Failed;
        if (rawPayloadJson is not null)
            RawPayloadJson = rawPayloadJson.Value;

        Touch(now);
    }

    // Refunds
    public Refund RequestRefund(JsonText? rawPayloadJson, DateTimeOffset now , string? reason = "")
    {
        if (Status != PaymentStatus.Captured)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.RefundNotCaptured);
        var refund = Refund.Create(Id , Amount , reason);
        _refunds.Add(refund);
        Touch(now);

        return refund;
    }

    public void UpdateRawPayload(JsonText rawPayloadJson, DateTimeOffset now)
    {
        RawPayloadJson = rawPayloadJson;
        Touch(now);
    }

    private void EnsureNotFinal()
    {
        if (Status is PaymentStatus.Failed or PaymentStatus.Refunded)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.StatusFinal);
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Payment.NowRequired);

        UpdatedAt = now;
    }
}

