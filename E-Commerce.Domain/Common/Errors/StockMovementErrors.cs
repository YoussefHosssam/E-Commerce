using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Common.Errors;

public static class StockMovementErrors
{
    public static readonly Error TypeInvalid =
        new(
            ErrorCodes.StockMovement.TypeInvalid,
            "Stock movement type is invalid.",
            ErrorType.Validation
        );

    public static readonly Error VariantIdRequired =
        new(
            ErrorCodes.StockMovement.VariantIdRequired,
            "Variant ID is required for stock movement.",
            ErrorType.Validation
        );

    public static readonly Error ReasonTooLong =
        new(
            ErrorCodes.StockMovement.ReasonTooLong,
            "Stock movement reason must not exceed the allowed length.",
            ErrorType.Validation
        );

    public static readonly Error QuantityDeltaInvalid =
        new(
            ErrorCodes.StockMovement.QuantityDeltaInvalid,
            "Quantity delta must not be zero.",
            ErrorType.Validation
        );

    public static readonly Error PositiveDeltaRequired =
        new(
            ErrorCodes.StockMovement.PositiveDeltaRequired,
            "Quantity delta must be positive for this stock movement type.",
            ErrorType.Validation
        );

    public static readonly Error NegativeDeltaRequired =
        new(
            ErrorCodes.StockMovement.NegativeDeltaRequired,
            "Quantity delta must be negative for this stock movement type.",
            ErrorType.Validation
        );

    public static readonly Error NowRequired =
        new(
            ErrorCodes.StockMovement.NowRequired,
            "Current date/time is required for stock movement.",
            ErrorType.Validation
        );
}