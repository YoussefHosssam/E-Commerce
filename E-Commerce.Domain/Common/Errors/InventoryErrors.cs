using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Common.Errors;

public static class InventoryErrors
{
    public static readonly Error VariantIdRequired =
        new(
            ErrorCodes.Inventory.VariantIdRequired,
            "Variant ID is required for inventory.",
            ErrorType.Validation
        );

    public static readonly Error InitialQuantityInvalid =
        new(
            ErrorCodes.Inventory.InitialQuantityInvalid,
            "Initial quantity must be zero or greater.",
            ErrorType.Validation
        );

    public static readonly Error QuantityInvalid =
        new(
            ErrorCodes.Inventory.QuantityInvalid,
            "Quantity value is invalid.",
            ErrorType.Validation
        );

    public static readonly Error InsufficientStock =
        new(
            ErrorCodes.Inventory.InsufficientStock,
            "Insufficient stock available for the requested quantity.",
            ErrorType.Conflict
        );

    public static readonly Error ReservedQuantityInvalid =
        new(
            ErrorCodes.Inventory.ReservedQuantityInvalid,
            "Reserved quantity is invalid.",
            ErrorType.Validation
        );

    public static readonly Error OnHandInvalid =
        new(
            ErrorCodes.Inventory.OnHandInvalid,
            "On-hand quantity is invalid.",
            ErrorType.Validation
        );

    public static readonly Error OnHandLessThanReserved =
        new(
            ErrorCodes.Inventory.OnHandLessThanReserved,
            "On-hand quantity cannot be less than reserved quantity.",
            ErrorType.Conflict
        );

    public static readonly Error NowRequired =
        new(
            ErrorCodes.Inventory.NowRequired,
            "Current date/time is required for inventory operation.",
            ErrorType.Validation
        );
}