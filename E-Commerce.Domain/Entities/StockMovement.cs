using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class StockMovement : BaseEntity
{
    public Guid VariantId { get; private set; }
    public Variant Variant { get; private set; } = default!;

    public StockMovementType Type { get; private set; }
    public int QuantityDelta { get; private set; }

    public string? Reason { get; private set; }

    public Guid? RefId { get; private set; }

    public Guid? ActorUserId { get; private set; }
    public User? ActorUser { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    private StockMovement() { } // EF

    private StockMovement(
        Guid variantId,
        StockMovementType type,
        int quantityDelta,
        string? reason,
        Guid? refId,
        Guid? actorUserId,
        DateTimeOffset now)
    {
        VariantId = variantId;
        Type = type;
        QuantityDelta = quantityDelta;
        Reason = reason;
        RefId = refId;
        ActorUserId = actorUserId;
        UpdatedAt = now;
    }

    public static StockMovement Create(
        Guid variantId,
        StockMovementType type,
        int quantityDelta,
        string? reason,
        Guid? refId,
        Guid? actorUserId,
        DateTimeOffset now)
    {
        if (variantId == Guid.Empty)
            throw new DomainValidationException(StockMovementErrors.VariantIdRequired);

        if (quantityDelta == 0)
            throw new DomainValidationException(StockMovementErrors.QuantityDeltaInvalid);

        if (now == default)
            throw new DomainValidationException(StockMovementErrors.NowRequired);

        reason = string.IsNullOrWhiteSpace(reason)
            ? null
            : reason.Trim();

        if (reason is not null && reason.Length > 250)
            throw new DomainValidationException(StockMovementErrors.ReasonTooLong);

        ValidateMovementTypeWithDelta(type, quantityDelta);

        return new StockMovement(
            variantId,
            type,
            quantityDelta,
            reason,
            refId,
            actorUserId,
            now);
    }

    private static void ValidateMovementTypeWithDelta(
        StockMovementType type,
        int quantityDelta)
    {
        switch (type)
        {
            case StockMovementType.InitialStock:
            case StockMovementType.StockIn:
            case StockMovementType.Return:
            case StockMovementType.ReservationReleased:
                if (quantityDelta < 0)
                    throw new DomainValidationException(StockMovementErrors.PositiveDeltaRequired);
                break;

            case StockMovementType.StockOut:
            case StockMovementType.Sale:
            case StockMovementType.Damage:
            case StockMovementType.Reservation:
                if (quantityDelta > 0)
                    throw new DomainValidationException(StockMovementErrors.NegativeDeltaRequired);
                break;

            case StockMovementType.Adjustment:
                break;

            default:
                throw new DomainValidationException(StockMovementErrors.TypeInvalid);
        }
    }
}