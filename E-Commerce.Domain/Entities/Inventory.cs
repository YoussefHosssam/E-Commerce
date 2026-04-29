using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class Inventory : BaseEntity
{
    public Guid VariantId { get; private set; }
    public Variant Variant { get; private set; } = default!;

    public int OnHand { get; private set; }
    public int Reserved { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    private Inventory() { } // EF

    private Inventory(Guid variantId, int onHand, DateTimeOffset now)
    {
        VariantId = variantId;
        OnHand = onHand;
        Reserved = 0;
        UpdatedAt = now;
    }

    public static Inventory Create(Guid variantId, int initialQuantity, DateTimeOffset now)
    {
        if (variantId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.VariantIdRequired);

        if (initialQuantity < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.InitialQuantityInvalid);

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.NowRequired);

        return new Inventory(variantId, initialQuantity, now);
    }

    public int Available => OnHand - Reserved;

    public void AddStock(int quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.QuantityInvalid);

        OnHand += quantity;
        Touch(now);
    }

    public void Reserve(int quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.QuantityInvalid);

        if (Available < quantity)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.InsufficientStock);

        Reserved += quantity;
        Touch(now);
    }

    public void ReleaseReservation(int quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.QuantityInvalid);

        if (Reserved < quantity)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.ReservedQuantityInvalid);

        Reserved -= quantity;
        Touch(now);
    }

    public void CommitReservation(int quantity, DateTimeOffset now)
    {
        if (quantity <= 0)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.QuantityInvalid);

        if (Reserved < quantity)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.ReservedQuantityInvalid);

        Reserved -= quantity;
        OnHand -= quantity;

        Touch(now);
    }

    public void AdjustStock(int newOnHand, DateTimeOffset now)
    {
        if (newOnHand < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.OnHandInvalid);

        if (newOnHand < Reserved)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.OnHandLessThanReserved);

        OnHand = newOnHand;
        Touch(now);
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Inventory.NowRequired);

        UpdatedAt = now;
    }
}