using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class PaymentAttempt : BaseEntity
{
    public Guid OrderId { get; private set; }

    public string Provider { get; private set; } = default!;
    public string? ProviderSessionId { get; private set; }
    public string? ProviderOrderId { get; private set; }
    public string? ProviderPaymentId { get; private set; }
    public string? PaymentUrl { get; private set; }
    public string IdempotencyKey { get; private set; } = default!;
    public string? RequestHash { get; private set; }
    public PaymentAttemptStatus Status { get; private set; } = PaymentAttemptStatus.Initiated;
    public decimal Amount { get; private set; }
    public CurrencyCode Currency { get; private set; } = CurrencyCode.Create("EGP");

    public JsonText RawPayloadJson { get; private set; } = JsonText.Create("{}");

    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private PaymentAttempt() { } // EF

    private PaymentAttempt(
        Guid orderId,
        string provider,
        decimal amount,
        CurrencyCode currency,
        string idempotencyKey,
        string? requestHash,
        JsonText rawPayloadJson,
        DateTimeOffset expiresAt,
        DateTimeOffset now)
    {
        OrderId = orderId;
        Provider = provider;
        Amount = amount;
        Currency = currency;
        IdempotencyKey = idempotencyKey;
        RequestHash = requestHash;
        RawPayloadJson = rawPayloadJson;
        ExpiresAt = expiresAt;
        CreatedAt = now;
        UpdatedAt = now;
        Status = PaymentAttemptStatus.Initiated;
    }

    public static PaymentAttempt Create(
        Guid orderId,
        string provider,
        decimal amount,
        CurrencyCode currency,
        string idempotencyKey,
        DateTimeOffset expiresAt,
        DateTimeOffset now,
        string? requestHash = null,
        JsonText? rawPayloadJson = null)
    {
        if (orderId == Guid.Empty)
            throw new DomainValidationException(PaymentAttemptErrors.OrderIdRequired);

        if (string.IsNullOrWhiteSpace(provider))
            throw new DomainValidationException(PaymentAttemptErrors.ProviderRequired);

        provider = provider.Trim().ToLowerInvariant();

        if (provider.Length > 30)
            throw new DomainValidationException(PaymentAttemptErrors.ProviderTooLong);

        if (amount <= 0)
            throw new DomainValidationException(PaymentAttemptErrors.AmountInvalid);

        if (currency.Equals(default(CurrencyCode)))
            throw new DomainValidationException(PaymentAttemptErrors.CurrencyRequired);

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new DomainValidationException(PaymentAttemptErrors.IdempotencyKeyRequired);

        idempotencyKey = idempotencyKey.Trim();

        if (idempotencyKey.Length > 120)
            throw new DomainValidationException(PaymentAttemptErrors.IdempotencyKeyTooLong);

        if (!string.IsNullOrWhiteSpace(requestHash) && requestHash.Length > 120)
            throw new DomainValidationException(PaymentAttemptErrors.RequestHashTooLong);

        if (now == default)
            throw new DomainValidationException(PaymentAttemptErrors.NowRequired);

        if (expiresAt <= now)
            throw new DomainValidationException(PaymentAttemptErrors.ExpiresAtInvalid);

        rawPayloadJson ??= JsonText.Create("{}");

        return new PaymentAttempt(
            orderId,
            provider,
            amount,
            currency,
            idempotencyKey,
            requestHash,
            rawPayloadJson.Value,
            expiresAt,
            now);
    }

    public void AttachProviderSession(
        string providerSessionId,
        string paymentUrl,
        DateTimeOffset now,
        JsonText? rawPayloadJson = null)
    {
        EnsureNotFinal();

        if (string.IsNullOrWhiteSpace(providerSessionId))
            throw new DomainValidationException(PaymentAttemptErrors.ProviderSessionIdRequired);

        if (string.IsNullOrWhiteSpace(paymentUrl))
            throw new DomainValidationException(PaymentAttemptErrors.PaymentUrlRequired);

        providerSessionId = providerSessionId.Trim();
        paymentUrl = paymentUrl.Trim();

        if (providerSessionId.Length > 120)
            throw new DomainValidationException(PaymentAttemptErrors.ProviderSessionIdTooLong);

        if (paymentUrl.Length > 2000)
            throw new DomainValidationException(PaymentAttemptErrors.PaymentUrlTooLong);

        ProviderSessionId = providerSessionId;
        PaymentUrl = paymentUrl;
        Status = PaymentAttemptStatus.AwaitingCustomerAction;

        if (rawPayloadJson is not null)
            RawPayloadJson = rawPayloadJson.Value;

        Touch(now);
    }
    public void AttachProviderOrderId(
        string providerOrderId,
        DateTimeOffset now)
    {
        EnsureNotFinal();

        if (string.IsNullOrWhiteSpace(providerOrderId))
            throw new DomainValidationException(PaymentAttemptErrors.OrderIdRequired);

        providerOrderId = providerOrderId.Trim();

        if (providerOrderId.Length > 120)
            throw new DomainValidationException(PaymentAttemptErrors.ProviderSessionIdTooLong);


        ProviderOrderId = providerOrderId;

        Touch(now);
    }
    public void AttachProviderPaymentId(string providerPaymentId, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(providerPaymentId))
            throw new DomainValidationException(PaymentAttemptErrors.ProviderPaymentIdRequired);

        providerPaymentId = providerPaymentId.Trim();

        if (providerPaymentId.Length > 120)
            throw new DomainValidationException(PaymentAttemptErrors.ProviderPaymentIdTooLong);

        ProviderPaymentId = providerPaymentId;
        Touch(now);
    }

    public void MarkPaid(DateTimeOffset now, JsonText? rawPayloadJson = null)
    {
        EnsureNotFinal();

        Status = PaymentAttemptStatus.Paid;

        if (rawPayloadJson is not null)
            RawPayloadJson = rawPayloadJson.Value;

        Touch(now);
    }

    public void MarkFailed(DateTimeOffset now, JsonText? rawPayloadJson = null)
    {
        EnsureNotFinal();

        Status = PaymentAttemptStatus.Failed;

        if (rawPayloadJson is not null)
            RawPayloadJson = rawPayloadJson.Value;

        Touch(now);
    }

    public void MarkExpired(DateTimeOffset now)
    {
        EnsureNotFinal();

        if (now < ExpiresAt)
            throw new DomainValidationException(PaymentAttemptErrors.NotExpiredYet);

        Status = PaymentAttemptStatus.Expired;
        Touch(now);
    }

    public void UpdateRawPayload(JsonText rawPayloadJson, DateTimeOffset now)
    {
        RawPayloadJson = rawPayloadJson;
        Touch(now);
    }

    public bool IsActive(DateTimeOffset now)
    {
        return Status is PaymentAttemptStatus.Initiated or PaymentAttemptStatus.AwaitingCustomerAction
               && ExpiresAt > now;
    }

    private void EnsureNotFinal()
    {
        if (Status is PaymentAttemptStatus.Paid or PaymentAttemptStatus.Failed or PaymentAttemptStatus.Expired)
            throw new DomainValidationException(PaymentAttemptErrors.StatusFinal);
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(PaymentAttemptErrors.NowRequired);

        UpdatedAt = now;
    }
}