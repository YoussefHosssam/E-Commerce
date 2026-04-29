namespace E_Commerce.Domain.Common.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new(ErrorCodes.Product.NotFound, "Product not found.", ErrorType.NotFound);
    public static readonly Error SlugAlreadyExists = new(ErrorCodes.Product.SlugDuplicate, "Product slug already exists.", ErrorType.Conflict);
    public static readonly Error HasVariants = new(ErrorCodes.Product.HasVariants, "Product cannot be deleted while it still has variants.", ErrorType.Conflict);
    public static readonly Error InvalidInput = new(ErrorCodes.Product.InvalidInput, "Product data is invalid.", ErrorType.Validation);
    public static readonly Error IdRequired = new(ErrorCodes.Product.IdRequired, "Product id is required.", ErrorType.Validation);
    public static readonly Error CategoryRequired = new(ErrorCodes.Product.CategoryRequired, "Category id is required.", ErrorType.Validation);
    public static readonly Error SlugRequired = new(ErrorCodes.Product.SlugRequired, "Slug is required.", ErrorType.Validation);
    public static readonly Error StatusInvalid = new(ErrorCodes.Product.StatusInvalid, "Product status is invalid.", ErrorType.Validation);
    public static readonly Error BasePriceRequired = new(ErrorCodes.Product.BasePriceRequired, "Base price is required.", ErrorType.Validation);
    public static readonly Error BasePriceInvalid = new(ErrorCodes.Product.BasePriceInvalid, "Base price cannot be negative.", ErrorType.Validation);
    public static readonly Error CurrencyRequired = new(ErrorCodes.Product.CurrencyRequired, "Currency is required.", ErrorType.Validation);
    public static readonly Error CurrencyInvalid = new(ErrorCodes.Product.CurrencyInvalid, "Currency code must be a 3-letter ISO code.", ErrorType.Validation);
    public static readonly Error BrandTooLong = new(ErrorCodes.Product.BrandTooLong, "Brand must not exceed 80 characters.", ErrorType.Validation);
}
