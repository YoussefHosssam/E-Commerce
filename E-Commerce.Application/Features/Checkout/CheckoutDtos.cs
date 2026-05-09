using E_Commerce.Domain.Entities;
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
        Guid PaymentAttemptId,
        string Provider,
        string? PaymentUrl,
        string? ClientSecret,
        DateTimeOffset ExpiresAt,
        bool IsReusedSession,
        PaymentAttemptStatus Status)
    {
        public static PaymentDto Created(
            Guid paymentAttemptId,
            string provider,
            string? paymentUrl,
            string? clientSecret,
            DateTimeOffset expiresAt)
        {
            return new PaymentDto(
                paymentAttemptId,
                provider,
                paymentUrl,
                clientSecret,
                expiresAt,
                IsReusedSession: false,
                Status: PaymentAttemptStatus.AwaitingCustomerAction);
        }

        public static PaymentDto FailedInitialization(
            string provider,
            Guid paymentAttemptId,
            DateTimeOffset expiresAt)
        {
            return new PaymentDto(
                paymentAttemptId,
                provider,
                PaymentUrl: null,
                ClientSecret: null,
                expiresAt,
                IsReusedSession: false,
                Status: PaymentAttemptStatus.Failed);
        }
    }
}