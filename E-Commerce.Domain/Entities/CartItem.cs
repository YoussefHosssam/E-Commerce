using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Cart Cart { get; private set; } = default!; // EF navigation

    public Guid VariantId { get; private set; }
    public Variant Variant { get; private set; } = default!; // EF navigation

    public int Quantity { get; private set; }

    public DateTimeOffset AddedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private CartItem() { } // EF

    private CartItem(Guid cartId, Guid variantId, int quantity, DateTimeOffset now)
    {
        CartId = cartId;
        VariantId = variantId;
        Quantity = quantity;

        AddedAt = now;
        UpdatedAt = null;
    }

    public static CartItem Create(Guid cartId, Guid variantId, int quantity, DateTimeOffset now)
    {
        if (cartId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.CartIdRequired);

        if (variantId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.VariantIdRequired);

        if (quantity <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.QuantityInvalid);

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.NowRequired);

        return new CartItem(cartId, variantId, quantity, now);
    }

    public void IncreaseQuantity(int delta, DateTimeOffset now)
    {
        if (delta <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.DeltaInvalid);

        Quantity += delta;
        Touch(now);
    }

    public void SetQuantity(int quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.QuantityInvalid);

        Quantity = quantity;
        Touch(now);
    }

    public void DecreaseQuantity(int delta, DateTimeOffset now)
    {
        if (delta <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.DeltaInvalid);

        var newQty = Quantity - delta;
        if (newQty <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.QuantityInvalid);

        Quantity = newQty;
        Touch(now);
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.CartItem.NowRequired);

        UpdatedAt = now;
    }
}

