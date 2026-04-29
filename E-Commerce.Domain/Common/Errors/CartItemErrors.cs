namespace E_Commerce.Domain.Common.Errors;

public static class CartItemErrors
{
    public static readonly Error CartIdRequired = new(ErrorCodes.Domain.CartItem.CartIdRequired, "Cart ID is required.", ErrorType.Validation);
    public static readonly Error DeltaInvalid = new(ErrorCodes.Domain.CartItem.DeltaInvalid, "Cart item delta is invalid.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Domain.CartItem.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error QuantityInvalid = new(ErrorCodes.Domain.CartItem.QuantityInvalid, "Cart item quantity is invalid.", ErrorType.Validation);
    public static readonly Error VariantIdRequired = new(ErrorCodes.Domain.CartItem.VariantIdRequired, "Variant ID is required.", ErrorType.Validation);
}
