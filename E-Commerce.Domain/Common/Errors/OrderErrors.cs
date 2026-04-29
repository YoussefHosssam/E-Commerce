namespace E_Commerce.Domain.Common.Errors;

public static class OrderErrors
{
    public static readonly Error BillingAddressRequired = new(ErrorCodes.Domain.Order.BillingAddressRequired, "Billing address is required.", ErrorType.Validation);
    public static readonly Error CancelNotAllowed = new(ErrorCodes.Domain.Order.CancelNotAllowed, "Order cancel is not allowed.", ErrorType.Conflict);
    public static readonly Error CancelReasonTooLong = new(ErrorCodes.Domain.Order.CancelReasonTooLong, "Cancel reason is too long.", ErrorType.Validation);
    public static readonly Error CurrencyRequired = new(ErrorCodes.Domain.Order.CurrencyRequired, "Order currency is required.", ErrorType.Validation);
    public static readonly Error DiscountInvalid = new(ErrorCodes.Domain.Order.DiscountInvalid, "Order discount is invalid.", ErrorType.Validation);
    public static readonly Error ItemCurrencyMismatch = new(ErrorCodes.Domain.Order.ItemCurrencyMismatch, "Order item currency does not match the order currency.", ErrorType.Conflict);
    public static readonly Error ItemNotFound = new(ErrorCodes.Domain.Order.ItemNotFound, "Order item not found.", ErrorType.NotFound);
    public static readonly Error ItemRequired = new(ErrorCodes.Domain.Order.ItemRequired, "Order item is required.", ErrorType.Validation);
    public static readonly Error NotEditable = new(ErrorCodes.Domain.Order.NotEditable, "Order is not editable.", ErrorType.Conflict);
    public static readonly Error NotesTooLong = new(ErrorCodes.Domain.Order.NotesTooLong, "Order notes are too long.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Domain.Order.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error NumberRequired = new(ErrorCodes.Domain.Order.NumberRequired, "Order number is required.", ErrorType.Validation);
    public static readonly Error NumberTooLong = new(ErrorCodes.Domain.Order.NumberTooLong, "Order number is too long.", ErrorType.Validation);
    public static readonly Error PaymentMismatch = new(ErrorCodes.Domain.Order.PaymentMismatch, "Order payment does not match.", ErrorType.Conflict);
    public static readonly Error PaymentRequired = new(ErrorCodes.Domain.Order.PaymentRequired, "Order payment is required.", ErrorType.Validation);
    public static readonly Error ShippingAddressRequired = new(ErrorCodes.Domain.Order.ShippingAddressRequired, "Shipping address is required.", ErrorType.Validation);
    public static readonly Error ShippingFeeInvalid = new(ErrorCodes.Domain.Order.ShippingFeeInvalid, "Shipping fee is invalid.", ErrorType.Validation);
    public static readonly Error StatusInvalidTransition = new(ErrorCodes.Domain.Order.StatusInvalidTransition, "Order status transition is invalid.", ErrorType.Conflict);
    public static readonly Error TaxInvalid = new(ErrorCodes.Domain.Order.TaxInvalid, "Order tax is invalid.", ErrorType.Validation);
    public static readonly Error TotalInvalid = new(ErrorCodes.Domain.Order.TotalInvalid, "Order total is invalid.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Domain.Order.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
