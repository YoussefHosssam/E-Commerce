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
    public static readonly Error SkuDuplicate = new(ErrorCodes.Variant.SkuDuplicate, "Sku already exists.", ErrorType.Validation);
    public static readonly Error ColorRequired = new(ErrorCodes.Variant.ColorRequired, "Variant color is required.", ErrorType.Validation);
    public static readonly Error ColorNameRequired = new(ErrorCodes.Variant.ColorNameRequired, "Color name is required.", ErrorType.Validation);
    public static readonly Error ColorHexCodeRequired = new(ErrorCodes.Variant.ColorHexCodeRequired, "Color hex code is required.", ErrorType.Validation);
    public static readonly Error ColorHexCodeInvalid = new(ErrorCodes.Variant.ColorHexCodeInvalid, "Color hex code format is invalid.", ErrorType.Validation);
    public static readonly Error ColorTooLong = new(ErrorCodes.Variant.ColorTooLong, "Variant color is too long.", ErrorType.Validation);
    public static readonly Error ImageRequired = new(ErrorCodes.Variant.ImageRequired, "Variant image is required.", ErrorType.Validation);
    public static readonly Error InventoryRequired = new(ErrorCodes.Variant.InventoryRequired, "Variant inventory is required.", ErrorType.Validation);
    public static readonly Error SizeTooLong = new(ErrorCodes.Variant.SizeTooLong, "Variant size is too long.", ErrorType.Validation);
    public static readonly Error DefaultVariantAlreadyExists = new(ErrorCodes.Variant.DefaultVariantAlreadyExists, "Default variant already exists for this product.", ErrorType.Conflict);
    public static readonly Error ProductMustHaveDefaultVariant = new(ErrorCodes.Variant.ProductMustHaveDefaultVariant, "Product must have a default variant.", ErrorType.Validation);
    public static readonly Error ProductMustHaveAtLeastOneVariant = new(ErrorCodes.Variant.ProductMustHaveAtLeastOneVariant, "Product must have at least one variant.", ErrorType.Validation);
    public static readonly Error SimpleProductMustHaveExactlyOneVariant = new(ErrorCodes.Variant.SimpleProductMustHaveExactlyOneVariant, "Simple product must have exactly one variant.", ErrorType.Validation);
    public static readonly Error VariantProductRequiresRealVariants = new(ErrorCodes.Variant.VariantProductRequiresRealVariants, "Configurable products require distinguishable variants.", ErrorType.Validation);
    public static readonly Error CannotDeleteLastActiveVariant = new(ErrorCodes.Variant.CannotDeleteLastActiveVariant, "Cannot delete the last active variant.", ErrorType.Conflict);
    public static readonly Error CannotDeleteVariantUsedInOrders = new(ErrorCodes.Variant.CannotDeleteVariantUsedInOrders, "Cannot delete a variant used in orders.", ErrorType.Conflict);
    public static readonly Error CannotDeleteVariantUsedInCart = new(ErrorCodes.Variant.CannotDeleteVariantUsedInCart, "Cannot delete a variant used in active carts.", ErrorType.Conflict);
    public static readonly Error DuplicateVariantOptions = new(ErrorCodes.Variant.DuplicateVariantOptions, "Duplicate variant option combination.", ErrorType.Validation);
    public static readonly Error VariantDoesNotBelongToProduct = new(ErrorCodes.Variant.VariantDoesNotBelongToProduct, "Variant does not belong to product.", ErrorType.Validation);

}
