using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Checkout
{
    public sealed record CheckoutSummaryDto(
        IReadOnlyCollection<CheckoutItemDto> Items,
        int TotalItems,
        int TotalQuantity,
        decimal Subtotal,
        decimal ShippingFee,
        decimal Total,
        string Currency);

    public sealed record CheckoutItemDto(
        Guid CartItemId,
        Guid VariantId,
        string Sku,
        string? Size,
        string? Color,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal,
        string Currency,
        string? ImageUrl);

    public sealed record PlaceOrderResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency,
    PaymentDto? Payment
);

    public sealed record PaymentDto(
        Uri PaymentUrl,
        DateTimeOffset ExpiresAt
    );
}
