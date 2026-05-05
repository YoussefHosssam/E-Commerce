using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Common.Errors;

public static class OrderErrors
{
    // ---------------- Root ----------------

    public static readonly Error CancelNotAllowed =
        new(ErrorCodes.Order.CancelNotAllowed, "Order cannot be cancelled in its current state.", ErrorType.Conflict);

    public static readonly Error NotEditable =
        new(ErrorCodes.Order.NotEditable, "Order cannot be modified in its current state.", ErrorType.Conflict);

    public static readonly Error StatusInvalidTransition =
        new(ErrorCodes.Order.StatusInvalidTransition, "Invalid order status transition.", ErrorType.Conflict);

    public static readonly Error UserIdRequired =
        new(ErrorCodes.Order.UserIdRequired, "User ID is required to create an order.", ErrorType.Validation);

    public static readonly Error NumberRequired =
        new(ErrorCodes.Order.NumberRequired, "Order number is required.", ErrorType.Validation);

    public static readonly Error NumberTooLong =
        new(ErrorCodes.Order.NumberTooLong, "Order number exceeds the allowed length.", ErrorType.Validation);

    public static readonly Error NowRequired =
        new(ErrorCodes.Order.NowRequired, "Current date/time is required.", ErrorType.Validation);

    public static readonly Error NotesTooLong =
        new(ErrorCodes.Order.NotesTooLong, "Order notes exceed the allowed length.", ErrorType.Validation);

    public static readonly Error CancelReasonTooLong =
        new(ErrorCodes.Order.CancelReasonTooLong, "Order cancel reason exceed the allowed length.", ErrorType.Validation);
    

    // ---------------- Pricing ----------------

    public static readonly Error CurrencyRequired =
        new(ErrorCodes.Order.CurrencyRequired, "Order currency is required.", ErrorType.Validation);

    public static readonly Error DiscountInvalid =
        new(ErrorCodes.Order.DiscountInvalid, "Order discount value is invalid.", ErrorType.Validation);

    public static readonly Error TaxInvalid =
        new(ErrorCodes.Order.TaxInvalid, "Order tax value is invalid.", ErrorType.Validation);

    public static readonly Error ShippingFeeInvalid =
        new(ErrorCodes.Order.ShippingFeeInvalid, "Shipping fee value is invalid.", ErrorType.Validation);

    public static readonly Error TotalInvalid =
        new(ErrorCodes.Order.TotalInvalid, "Order total amount is invalid.", ErrorType.Validation);

    // ---------------- Items ----------------

    public static readonly Error ItemRequired =
        new(ErrorCodes.Order.ItemRequired, "Order must contain at least one item.", ErrorType.Validation);

    public static readonly Error ItemNotFound =
        new(ErrorCodes.Order.ItemNotFound, "Order item was not found.", ErrorType.NotFound);

    public static readonly Error ItemCurrencyMismatch =
        new(ErrorCodes.Order.ItemCurrencyMismatch, "Order item currency must match the order currency.", ErrorType.Conflict);

    // ---------------- Payment ----------------

    public static readonly Error PaymentRequired =
        new(ErrorCodes.Order.PaymentRequired, "Order payment information is required.", ErrorType.Validation);

    public static readonly Error PaymentMismatch =
        new(ErrorCodes.Order.PaymentMismatch, "Payment details do not match the order.", ErrorType.Conflict);

    // ---------------- Address ----------------

    public static class ShippingAddress
    {
        public static readonly Error Required =
            new(ErrorCodes.Order.ShippingAddressRequired, "Shipping address is required.", ErrorType.Validation);

        public static readonly Error FullNameRequired =
            new(ErrorCodes.Order.ShippingAddressRequired, "Shipping full name is required.", ErrorType.Validation);

        public static readonly Error PhoneRequired =
            new(ErrorCodes.Order.ShippingAddressRequired, "Shipping phone number is required.", ErrorType.Validation);

        public static readonly Error CityRequired =
            new(ErrorCodes.Order.ShippingAddressRequired, "Shipping city is required.", ErrorType.Validation);

        public static readonly Error AddressLine1Required =
            new(ErrorCodes.Order.ShippingAddressRequired, "Shipping address line is required.", ErrorType.Validation);
    }

    public static class BillingAddress
    {
        public static readonly Error Required =
            new(ErrorCodes.Order.BillingAddressRequired, "Billing address is required.", ErrorType.Validation);

        public static readonly Error FullNameRequired =
            new(ErrorCodes.Order.BillingAddressRequired, "Billing full name is required.", ErrorType.Validation);

        public static readonly Error PhoneRequired =
            new(ErrorCodes.Order.BillingAddressRequired, "Billing phone number is required.", ErrorType.Validation);

        public static readonly Error CityRequired =
            new(ErrorCodes.Order.BillingAddressRequired, "Billing city is required.", ErrorType.Validation);

        public static readonly Error AddressLine1Required =
            new(ErrorCodes.Order.BillingAddressRequired, "Billing address line is required.", ErrorType.Validation);
    }
}