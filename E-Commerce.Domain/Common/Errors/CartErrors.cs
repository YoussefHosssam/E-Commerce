namespace E_Commerce.Domain.Common.Errors;

public static class CartErrors
{
    public static readonly Error AnonymousTokenRequired = new(ErrorCodes.Domain.Cart.AnonymousTokenRequired, "Anonymous token is required.", ErrorType.Validation);
    public static readonly Error AnonymousTokenTooLong = new(ErrorCodes.Domain.Cart.AnonymousTokenTooLong, "Anonymous token is too long.", ErrorType.Validation);
    public static readonly Error ItemMismatch = new(ErrorCodes.Domain.Cart.ItemMismatch, "Cart item does not belong to this cart.", ErrorType.Conflict);
    public static readonly Error ItemNotFound = new(ErrorCodes.Domain.Cart.ItemNotFound, "Cart item not found.", ErrorType.NotFound);
    public static readonly Error ItemRequired = new(ErrorCodes.Domain.Cart.ItemRequired, "Cart item is required.", ErrorType.Validation);
    public static readonly Error NotActive = new(ErrorCodes.Domain.Cart.NotActive, "Cart is not active.", ErrorType.Conflict);
    public static readonly Error NowRequired = new(ErrorCodes.Domain.Cart.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error StatusInvalid = new(ErrorCodes.Domain.Cart.StatusInvalid, "Cart status is invalid.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Domain.Cart.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
