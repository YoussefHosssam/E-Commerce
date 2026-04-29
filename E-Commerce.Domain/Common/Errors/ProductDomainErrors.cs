namespace E_Commerce.Domain.Common.Errors;

public static class ProductDomainErrors
{
    public static readonly Error ImageRequired = new(ErrorCodes.Domain.Product.ImageRequired, "Product image is required.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Domain.Product.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error VariantNotFound = new(ErrorCodes.Domain.Product.VariantNotFound, "Product variant not found.", ErrorType.NotFound);
    public static readonly Error VariantPriceOverrideCurrencyMismatch = new(ErrorCodes.Domain.Product.VariantPriceOverrideCurrencyMismatch, "Variant price override currency mismatch.", ErrorType.Conflict);
    public static readonly Error VariantPriceOverrideInvalid = new(ErrorCodes.Domain.Product.VariantPriceOverrideInvalid, "Variant price override is invalid.", ErrorType.Validation);
    public static readonly Error VariantSkuDuplicate = new(ErrorCodes.Domain.Product.VariantSkuDuplicate, "Variant SKU already exists.", ErrorType.Conflict);
    public static readonly Error VariantSkuRequired = new(ErrorCodes.Domain.Product.VariantSkuRequired, "Variant SKU is required.", ErrorType.Validation);
}
