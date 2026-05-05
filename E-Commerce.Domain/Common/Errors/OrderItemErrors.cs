namespace E_Commerce.Domain.Common.Errors;

public static class OrderItemErrors
{
    public static readonly Error CurrencyInvalid = new(ErrorCodes.OrderItem.CurrencyInvalid, "Order item currency is invalid.", ErrorType.Validation);
    public static readonly Error LineTotalInvalid = new(ErrorCodes.OrderItem.LineTotalInvalid, "Order item line total is invalid.", ErrorType.Validation);
    public static readonly Error OrderIdRequired = new(ErrorCodes.OrderItem.OrderIdRequired, "Order ID is required.", ErrorType.Validation);
    public static readonly Error QuantityInvalid = new(ErrorCodes.OrderItem.QuantityInvalid, "Order item quantity is invalid.", ErrorType.Validation);
    public static readonly Error SkuRequired = new(ErrorCodes.OrderItem.SkuRequired, "SKU is required.", ErrorType.Validation);
    public static readonly Error SkuTooLong = new(ErrorCodes.OrderItem.SkuTooLong, "SKU is too long.", ErrorType.Validation);
    public static readonly Error TitleRequired = new(ErrorCodes.OrderItem.TitleRequired, "Title is required.", ErrorType.Validation);
    public static readonly Error TitleTooLong = new(ErrorCodes.OrderItem.TitleTooLong, "Title is too long.", ErrorType.Validation);
    public static readonly Error UnitPriceInvalid = new(ErrorCodes.OrderItem.UnitPriceInvalid, "Unit price is invalid.", ErrorType.Validation);
    public static readonly Error VariantIdRequired = new(ErrorCodes.OrderItem.VariantIdRequired, "Variant ID is required.", ErrorType.Validation);
    public static readonly Error VariantSnapshotRequired = new(ErrorCodes.OrderItem.VariantSnapshotRequired, "Variant snapshot is required.", ErrorType.Validation);
}
