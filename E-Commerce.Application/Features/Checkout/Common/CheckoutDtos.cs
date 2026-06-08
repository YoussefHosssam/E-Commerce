using E_Commerce.Domain.Enums;

namespace E_Commerce.Application.Features.Checkout.Common;

public sealed record CheckoutSummaryDto
{
    public IReadOnlyCollection<CheckoutItemDto> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int TotalQuantity { get; init; }
    public decimal Subtotal { get; init; }
    public decimal ShippingFee { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = default!;
}

public sealed record CheckoutReviewDto(
    IReadOnlyCollection<CheckoutItemDto> Items,
    int TotalItems,
    int TotalQuantity,
    decimal Subtotal,
    decimal ShippingFee,
    decimal Total,
    string Currency,
    CheckoutAddressDto ShippingAddress);

public sealed record CheckoutAddressDto
{
    public Guid AddressId { get; init; }
    public string Label { get; init; } = default!;
    public string Country { get; init; } = default!;
    public string Governorate { get; init; } = default!;
    public string City { get; init; } = default!;
    public string Area { get; init; } = default!;
    public string Street { get; init; } = default!;
    public string BuildingNumber { get; init; } = default!;
    public string? Floor { get; init; }
    public string? Apartment { get; init; }
    public string? PostalCode { get; init; }
    public string? Landmark { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public bool IsDefault { get; init; }
}

public sealed record CheckoutItemDto
{
    public Guid CartItemId { get; init; }
    public Guid VariantId { get; init; }
    public string Sku { get; init; } = default!;
    public string? Size { get; init; }
    public string? Color { get; init; }
    public string ProductName { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
    public string Currency { get; init; } = default!;
    public string? ImageUrl { get; init; }
}

public sealed record PlaceOrderResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency,
    PaymentDto? Payment);

public sealed record PaymentDto
{
    public Guid PaymentAttemptId { get; init; }
    public string Provider { get; init; } = default!;
    public string? PaymentUrl { get; init; }
    public string? ClientSecret { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsReusedSession { get; init; }
    public PaymentAttemptStatus Status { get; init; }

    public static PaymentDto Created(
        Guid paymentAttemptId,
        string provider,
        string? paymentUrl,
        string? clientSecret,
        DateTimeOffset expiresAt)
    {
        return new PaymentDto
        {
            PaymentAttemptId = paymentAttemptId,
            Provider = provider,
            PaymentUrl = paymentUrl,
            ClientSecret = clientSecret,
            ExpiresAt = expiresAt,
            IsReusedSession = false,
            Status = PaymentAttemptStatus.AwaitingCustomerAction
        };
    }

    public static PaymentDto Reused(
        Guid paymentAttemptId,
        string provider,
        string? paymentUrl,
        string? clientSecret,
        DateTimeOffset expiresAt,
        PaymentAttemptStatus status)
    {
        return new PaymentDto
        {
            PaymentAttemptId = paymentAttemptId,
            Provider = provider,
            PaymentUrl = paymentUrl,
            ClientSecret = clientSecret,
            ExpiresAt = expiresAt,
            IsReusedSession = true,
            Status = status
        };
    }

    public static PaymentDto FailedInitialization(
        string provider,
        Guid paymentAttemptId,
        DateTimeOffset expiresAt)
    {
        return new PaymentDto
        {
            PaymentAttemptId = paymentAttemptId,
            Provider = provider,
            PaymentUrl = null,
            ClientSecret = null,
            ExpiresAt = expiresAt,
            IsReusedSession = false,
            Status = PaymentAttemptStatus.Failed
        };
    }
}
