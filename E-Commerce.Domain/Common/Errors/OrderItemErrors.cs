namespace E_Commerce.Domain.Common.Errors;

public static class OrderItemErrors
{
    public static readonly Error CurrencyInvalid = new(ErrorCodes.Domain.OrderItem.CurrencyInvalid, "Order item currency is invalid.", ErrorType.Validation);
    public static readonly Error LineTotalInvalid = new(ErrorCodes.Domain.OrderItem.LineTotalInvalid, "Order item line total is invalid.", ErrorType.Validation);
    public static readonly Error OrderIdRequired = new(ErrorCodes.Domain.OrderItem.OrderIdRequired, "Order ID is required.", ErrorType.Validation);
    public static readonly Error QuantityInvalid = new(ErrorCodes.Domain.OrderItem.QuantityInvalid, "Order item quantity is invalid.", ErrorType.Validation);
    public static readonly Error SkuRequired = new(ErrorCodes.Domain.OrderItem.SkuRequired, "SKU is required.", ErrorType.Validation);
    public static readonly Error SkuTooLong = new(ErrorCodes.Domain.OrderItem.SkuTooLong, "SKU is too long.", ErrorType.Validation);
    public static readonly Error TitleRequired = new(ErrorCodes.Domain.OrderItem.TitleRequired, "Title is required.", ErrorType.Validation);
    public static readonly Error TitleTooLong = new(ErrorCodes.Domain.OrderItem.TitleTooLong, "Title is too long.", ErrorType.Validation);
    public static readonly Error UnitPriceInvalid = new(ErrorCodes.Domain.OrderItem.UnitPriceInvalid, "Unit price is invalid.", ErrorType.Validation);
    public static readonly Error VariantIdRequired = new(ErrorCodes.Domain.OrderItem.VariantIdRequired, "Variant ID is required.", ErrorType.Validation);
    public static readonly Error VariantSnapshotRequired = new(ErrorCodes.Domain.OrderItem.VariantSnapshotRequired, "Variant snapshot is required.", ErrorType.Validation);
}
