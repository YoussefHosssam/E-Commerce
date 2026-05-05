namespace E_Commerce.Domain.Common.Errors;

public static class CartErrors
{
    public static readonly Error AnonymousTokenRequired = new(ErrorCodes.Cart.AnonymousTokenRequired, "Anonymous token is required.", ErrorType.Validation);
    public static readonly Error AnonymousTokenTooLong = new(ErrorCodes.Cart.AnonymousTokenTooLong, "Anonymous token is too long.", ErrorType.Validation);
    public static readonly Error ItemMismatch = new(ErrorCodes.Cart.ItemMismatch, "Cart item does not belong to this cart.", ErrorType.Conflict);
    public static readonly Error ItemNotFound = new(ErrorCodes.Cart.ItemNotFound, "Cart item not found.", ErrorType.NotFound);
    public static readonly Error ItemRequired = new(ErrorCodes.Cart.ItemRequired, "Cart item is required.", ErrorType.Validation);
    public static readonly Error NotActive = new(ErrorCodes.Cart.NotActive, "Cart is not active.", ErrorType.Conflict);
    public static readonly Error NowRequired = new(ErrorCodes.Cart.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error StatusInvalid = new(ErrorCodes.Cart.StatusInvalid, "Cart status is invalid.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Cart.UserIdRequired, "User ID is required.", ErrorType.Validation);
    public static readonly Error ItemsLimitExceeded = new(ErrorCodes.Cart.ItemsLimitExceeded, "You have exceeded the maximum allowed number of items in the cart.",ErrorType.Conflict);

}
