using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = default!; // EF navigation

    public Guid VariantId { get; private set; }
    public Variant Variant { get; private set; } = default!; // EF navigation

    public string Sku { get; private set; } = default!;
    public string ProductTitleSnapshot { get; private set; } = default!;
    public JsonText VariantSnapshotJson { get; private set; } = JsonText.Create("{}");

    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal { get; private set; }
    public CurrencyCode Currency { get; private set; }

    private OrderItem() { } // EF

    private OrderItem(
        Guid orderId,
        Guid variantId,
        string sku,
        string productTitleSnapshot,
        JsonText variantSnapshotJson,
        decimal unitPrice,
        CurrencyCode currency,
        int quantity)
    {
        OrderId = orderId;
        VariantId = variantId;
        Sku = sku;
        ProductTitleSnapshot = productTitleSnapshot;
        VariantSnapshotJson = variantSnapshotJson;
        Currency = currency;
        UnitPrice = unitPrice;
        Quantity = quantity;

        LineTotal = unitPrice * quantity;
    }

    public static OrderItem Create(
        Guid orderId,
        Guid variantId,
        CurrencyCode currency,
        string sku,
        string productTitleSnapshot,
        JsonText variantSnapshotJson,
        decimal unitPrice,
        int quantity)
    {
        if (orderId == Guid.Empty)
            throw new DomainValidationException(OrderItemErrors.OrderIdRequired);

        if (variantId == Guid.Empty)
            throw new DomainValidationException(OrderItemErrors.VariantIdRequired);

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainValidationException(OrderItemErrors.SkuRequired);

        sku = sku.Trim().ToUpperInvariant();
        if (sku.Length > 64)
            throw new DomainValidationException(OrderItemErrors.SkuTooLong);

        if (string.IsNullOrWhiteSpace(productTitleSnapshot))
            throw new DomainValidationException(OrderItemErrors.TitleRequired);

        productTitleSnapshot = productTitleSnapshot.Trim();
        if (productTitleSnapshot.Length > 200)
            throw new DomainValidationException(OrderItemErrors.TitleTooLong);

        if (variantSnapshotJson.Equals(default(JsonText)))
            throw new DomainValidationException(OrderItemErrors.VariantSnapshotRequired);

        if (unitPrice < 0)
            throw new DomainValidationException(OrderItemErrors.UnitPriceInvalid);

        if (currency.Equals(default(CurrencyCode)))
            throw new DomainValidationException(OrderItemErrors.CurrencyInvalid);

        if (quantity <= 0)
            throw new DomainValidationException(OrderItemErrors.QuantityInvalid);

        // safeguard: line total overflow-ish (???????)
        var lineTotal = unitPrice * quantity;
        if (lineTotal < 0)
            throw new DomainValidationException(OrderItemErrors.LineTotalInvalid);

        return new OrderItem(orderId, variantId, sku, productTitleSnapshot, variantSnapshotJson, unitPrice, currency, quantity);
    }

    public void ChangeQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException(OrderItemErrors.QuantityInvalid);

        Quantity = quantity;
        LineTotal = UnitPrice * Quantity;
    }

    public void ChangeUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new DomainValidationException(OrderItemErrors.UnitPriceInvalid);

        UnitPrice = unitPrice;
        LineTotal = UnitPrice * Quantity;
    }
}

