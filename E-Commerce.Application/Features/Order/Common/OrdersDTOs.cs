using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Order.Common
{
    public record OrderDto
    {
        public Guid Id { get; init; }
        public string OrderNumber { get; init; } = default!;
        public OrderStatus Status { get; init; } = default!;

        public decimal Subtotal { get; init; }
        public decimal ShippingFee { get; init; }
        public decimal DiscountTotal { get; init; }
        public decimal TaxTotal { get; init; }
        public decimal GrandTotal { get; init; }

        public string Currency { get; init; } = default!;

        public string ShippingAddress { get; init; } = default!;
        public string BillingAddress { get; init; } = default!;

        public string? Notes { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }

        public List<OrderItemDto> Items { get; init; } = new();
    }
    public record OrderItemDto
    {
        public Guid Id { get; init; }
        public string Sku { get; init; } = default!;
        public string ProductTitle { get; init; } = default!;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
        public decimal LineTotal { get; init; }
        public string Currency { get; init; } = default!;
    }
    public record OrderListDto
    {
        public Guid Id { get; init; }
        public string OrderNumber { get; init; } = default!;
        public OrderStatus Status { get; init; } = default!;
        public decimal GrandTotal { get; init; }
        public string Currency { get; init; } = default!;
        public DateTimeOffset? UpdatedAt { get; init; }
    }


}
