using E_Commerce.Domain.ValueObjects;
using System.Text.Json.Serialization;

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
    Money Money,
    int Quantity,
    string? ImageUrl = null,
    string? Description = null);

public record PaymentBillingDataDto(
    [property: JsonPropertyName("first_name")]
    string FirstName,

    [property: JsonPropertyName("last_name")]
    string LastName,

    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("phone_number")]
    string PhoneNumber,

    [property: JsonPropertyName("street")]
    string Street,

    [property: JsonPropertyName("building")]
    string Building,

    [property: JsonPropertyName("floor")]
    string Floor,

    [property: JsonPropertyName("apartment")]
    string Apartment,

    [property: JsonPropertyName("city")]
    string City,

    [property: JsonPropertyName("state")]
    string State,

    [property: JsonPropertyName("country")]
    string Country,

    [property: JsonPropertyName("postal_code")]
    string PostalCode
);
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
    decimal ShippingFee,
    DateTimeOffset ExpiresAt);
public sealed record CreateProviderPaymentSessionResult(
    string Provider,
    string? ProviderOrderId,
    string? ProviderSessionId,
    string? PaymentUrl,
    string? ClientSecret,
    string RawPayloadJson);