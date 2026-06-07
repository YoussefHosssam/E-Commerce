namespace E_Commerce.Domain.Common.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new(ErrorCodes.Product.NotFound, "Product not found.", ErrorType.NotFound);
    public static readonly Error SlugAlreadyExists = new(ErrorCodes.Product.SlugDuplicate, "Product slug already exists.", ErrorType.Conflict);
    public static readonly Error HasVariants = new(ErrorCodes.Product.HasVariants, "Product cannot be deleted while it still has variants.", ErrorType.Conflict);
    public static readonly Error InvalidInput = new(ErrorCodes.Product.InvalidInput, "Product data is invalid.", ErrorType.Validation);
    public static readonly Error IdRequired = new(ErrorCodes.Product.IdRequired, "Product id is required.", ErrorType.Validation);
    public static readonly Error CategoryRequired = new(ErrorCodes.Product.CategoryRequired, "Category id is required.", ErrorType.Validation);
    public static readonly Error NameInvalid = new(ErrorCodes.Product.NameInvalid, "Product name is invalid.", ErrorType.Validation);
    public static readonly Error SlugRequired = new(ErrorCodes.Product.SlugRequired, "Slug is required.", ErrorType.Validation);
    public static readonly Error StatusInvalid = new(ErrorCodes.Product.StatusInvalid, "Product status is invalid.", ErrorType.Validation);
    public static readonly Error BasePriceRequired = new(ErrorCodes.Product.BasePriceRequired, "Base price is required.", ErrorType.Validation);
    public static readonly Error BasePriceInvalid = new(ErrorCodes.Product.BasePriceInvalid, "Base price cannot be negative.", ErrorType.Validation);
    public static readonly Error CurrencyRequired = new(ErrorCodes.Product.CurrencyRequired, "Currency is required.", ErrorType.Validation);
    public static readonly Error CurrencyInvalid = new(ErrorCodes.Product.CurrencyInvalid, "Currency code must be a 3-letter ISO code.", ErrorType.Validation);
    public static readonly Error BrandTooLong = new(ErrorCodes.Product.BrandTooLong, "Brand must not exceed 80 characters.", ErrorType.Validation);
    public static readonly Error SlugDuplicate = new(ErrorCodes.Product.SlugDuplicate, "Slug already exists.", ErrorType.Validation);
    public static readonly Error ImageRequired = new(ErrorCodes.Product.ImageRequired, "Product image is required.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Product.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error VariantNotFound = new(ErrorCodes.Product.VariantNotFound, "Product variant not found.", ErrorType.NotFound);
    public static readonly Error VariantPriceOverrideCurrencyMismatch = new(ErrorCodes.Product.VariantPriceOverrideCurrencyMismatch, "Variant price override currency mismatch.", ErrorType.Conflict);
    public static readonly Error VariantPriceOverrideInvalid = new(ErrorCodes.Product.VariantPriceOverrideInvalid, "Variant price override is invalid.", ErrorType.Validation);
    public static readonly Error VariantSkuDuplicate = new(ErrorCodes.Product.VariantSkuDuplicate, "Variant SKU already exists.", ErrorType.Conflict);
    public static readonly Error VariantSkuRequired = new(ErrorCodes.Product.VariantSkuRequired, "Variant SKU is required.", ErrorType.Validation);
    public static readonly Error DiscountPriceRequired = new(ErrorCodes.Product.DiscountPriceRequired, "Discount compare-at price is required.", ErrorType.Validation);
    public static readonly Error DiscountPriceMustBeGreaterThanActualPrice = new(ErrorCodes.Product.DiscountPriceMustBeGreaterThanActualPrice, "Discount compare-at price must be greater than the actual selling price.", ErrorType.Validation);
    public static readonly Error DiscountPriceNotAllowedWhenHasDiscountFalse = new(ErrorCodes.Product.DiscountPriceNotAllowedWhenHasDiscountFalse, "Compare-at price is not allowed when product has no discount.", ErrorType.Validation);
    public static readonly Error InvalidDiscountState = new(ErrorCodes.Product.InvalidDiscountState, "Product discount state is invalid.", ErrorType.Validation);
    public static readonly Error ProductMustHaveAtLeastOneVariant = new(ErrorCodes.Product.ProductMustHaveAtLeastOneVariant, "Product must have at least one variant.", ErrorType.Validation);
    public static readonly Error SimpleProductMustHaveExactlyOneVariant = new(ErrorCodes.Product.SimpleProductMustHaveExactlyOneVariant, "Simple product must have exactly one variant.", ErrorType.Validation);
    public static readonly Error ProductMustHaveOneDefaultVariant = new(ErrorCodes.Product.ProductMustHaveOneDefaultVariant, "Product must have exactly one default variant.", ErrorType.Validation);
    public static readonly Error CannotAddVariantsToSimpleProduct = new(ErrorCodes.Product.CannotAddVariantsToSimpleProduct, "Product is configured as simple product. Convert it to variant product first.", ErrorType.Validation);

}
