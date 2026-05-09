using E_Commerce.Domain.ValueObjects;

public sealed record ProviderWebhookEvent(
    string Provider,
    string EventType,
    string ProviderTransactionId,
    string? ProviderOrderId,
    string? ProviderSessionId,
    string? MerchantOrderId,
    bool Success,
    bool Pending,
    bool IsRefund,
    bool IsVoid,
    bool IsAuth,
    bool IsCapture,
    decimal Amount,
    CurrencyCode Currency,
    string RawPayloadJson);


public sealed record PaymentSessionItemDto(
    string Name,
    decimal Amount,
    int Quantity);

public record PaymentBillingDataDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Street,
    string Building,
    string Floor,
    string Apartment,
    string City,
    string State,
    string Country,
    string PostalCode);
public sealed record CreateProviderPaymentSessionRequest(
    Guid OrderId,
    Guid PaymentAttemptId,
    string OrderNumber,
    decimal Amount,
    CurrencyCode Currency,
    IReadOnlyCollection<PaymentSessionItemDto> Items,
    PaymentBillingDataDto BillingData,
    string IdempotencyKey,
    string SpecialReference,
    DateTimeOffset ExpiresAt);
public sealed record CreateProviderPaymentSessionResult(
    string Provider,
    string? ProviderOrderId,
    string? ProviderSessionId,
    string? PaymentUrl,
    string? ClientSecret,
    string RawPayloadJson);