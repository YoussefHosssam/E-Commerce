namespace E_Commerce.Domain.Common.Errors;

public static class VariantErrors
{
    public static readonly Error NotFound = new(ErrorCodes.Variant.NotFound, "Variant not found.", ErrorType.NotFound);
    public static readonly Error SkuAlreadyExists = new(ErrorCodes.Variant.SkuDuplicate, "Variant SKU already exists.", ErrorType.Conflict);
    public static readonly Error DeleteReferenced = new(ErrorCodes.Variant.DeleteReferenced, "Variant cannot be deleted because it is referenced by other records.", ErrorType.Conflict);
    public static readonly Error InvalidInput = new(ErrorCodes.Variant.InvalidInput, "Variant data is invalid.", ErrorType.Validation);
    public static readonly Error ProductRequired = new(ErrorCodes.Variant.ProductRequired, "Product id is required.", ErrorType.Validation);
    public static readonly Error VariantIdRequired = new(ErrorCodes.Variant.VariantIdRequired, "Variant id is required.", ErrorType.Validation);
    public static readonly Error SkuRequired = new(ErrorCodes.Variant.SkuRequired, "SKU is required.", ErrorType.Validation);
    public static readonly Error SkuTooLong = new(ErrorCodes.Variant.SkuTooLong, "SKU must not exceed 64 characters.", ErrorType.Validation);
    public static readonly Error PriceInvalid = new(ErrorCodes.Variant.PriceInvalid, "Price override cannot be negative.", ErrorType.Validation);
    public static readonly Error PriceRequired = new(ErrorCodes.Variant.PriceRequired, "Price override amount and currency must be provided together.", ErrorType.Validation);
    public static readonly Error CurrencyRequired = new(ErrorCodes.Variant.CurrencyRequired, "Currency is required.", ErrorType.Validation);
    public static readonly Error CurrencyInvalid = new(ErrorCodes.Variant.CurrencyInvalid, "Currency code must be a 3-letter ISO code.", ErrorType.Validation);
}
