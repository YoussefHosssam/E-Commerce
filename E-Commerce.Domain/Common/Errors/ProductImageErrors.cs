namespace E_Commerce.Domain.Common.Errors;

public static class ProductImageErrors
{
    public static readonly Error ProductIdEmpty = new(ErrorCodes.ProductImage.ProductIdEmpty, "Product image product ID is empty.", ErrorType.Validation);
    public static readonly Error UrlEmpty = new(ErrorCodes.ProductImage.UrlEmpty, "Product image URL is empty.", ErrorType.Validation);
    public static readonly Error StorageKeyEmpty = new(ErrorCodes.ProductImage.StorageKeyEmpty, "Product image storage key is empty.", ErrorType.Validation);
    public static readonly Error WidthInvalid = new(ErrorCodes.ProductImage.WidthInvalid, "Product image width is invalid.", ErrorType.Validation);
    public static readonly Error HeightInvalid = new(ErrorCodes.ProductImage.HeightInvalid, "Product image height is invalid.", ErrorType.Validation);
    public static readonly Error SizeInvalid = new(ErrorCodes.ProductImage.SizeInvalid, "Product image size is invalid.", ErrorType.Validation);
    public static readonly Error FormatEmpty = new(ErrorCodes.ProductImage.FormatEmpty, "Product image format is empty.", ErrorType.Validation);
    public static readonly Error SortOrderInvalid = new(ErrorCodes.ProductImage.SortOrderInvalid, "Product image sort order is invalid.", ErrorType.Validation);
    public static readonly Error InvalidProcessingStatus = new(ErrorCodes.ProductImage.InvalidProcessingStatus, "Product image processing status is invalid.", ErrorType.Validation);
    public static readonly Error ImageRequired = new(ErrorCodes.ProductImage.ImageRequired, "Product image is required.", ErrorType.Validation);
    public static readonly Error ImageNotFound = new(ErrorCodes.ProductImage.ImageNotFound, "Product image was not found.", ErrorType.NotFound);
}
