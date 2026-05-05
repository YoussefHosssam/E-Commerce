using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Common.Errors;

public static class CheckoutErrors
{
    public static readonly Error EmptyCart =
        new(
            ErrorCodes.Checkout.EmptyCart,
            "Your cart is empty.",
            ErrorType.Validation
        );

    public static readonly Error InActiveProduct =
        new(
            ErrorCodes.Checkout.InActiveProduct,
            "One or more items in your cart are no longer available for purchase.",
            ErrorType.Conflict
        );

    public static readonly Error UnfoundInventory =
        new(
            ErrorCodes.Checkout.UnfoundInventory,
            "Inventory information for one or more items could not be found.",
            ErrorType.NotFound
        );

    public static readonly Error VariantOutOfStock =
        new(
            ErrorCodes.Checkout.VariantOutOfStock,
            "Requested quantity is not available in stock.",
            ErrorType.Conflict
        );

    public static readonly Error QuantityInvalid =
        new(
            ErrorCodes.Checkout.QuantityInvalid,
            "One or more items in your cart have invalid quantity.",
            ErrorType.Validation
        );
}