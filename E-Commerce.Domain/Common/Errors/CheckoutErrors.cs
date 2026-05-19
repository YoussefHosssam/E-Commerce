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

    public static readonly Error DefaultAddressNotFound =
        new(
            ErrorCodes.Checkout.DefaultAddressNotFound,
            "Default shipping address was not found.",
            ErrorType.NotFound
        );

    public static readonly Error AddressRequired =
        new(
            ErrorCodes.Checkout.AddressRequired,
            "Shipping address is required.",
            ErrorType.Validation
        );

    public static readonly Error AddressDoesNotBelongToUser =
        new(
            ErrorCodes.Checkout.AddressDoesNotBelongToUser,
            "Shipping address does not belong to the current user.",
            ErrorType.Forbidden
        );

    public static readonly Error AddressInvalidForShipping =
        new(
            ErrorCodes.Checkout.AddressInvalidForShipping,
            "Shipping address is missing required shipping information.",
            ErrorType.Validation
        );

    public static readonly Error ShipmentFeeCalculationFailed =
        new(
            ErrorCodes.Checkout.ShipmentFeeCalculationFailed,
            "Shipment fee could not be calculated for the selected address.",
            ErrorType.External
        );
}
