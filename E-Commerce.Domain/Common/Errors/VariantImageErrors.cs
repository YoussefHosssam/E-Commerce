namespace E_Commerce.Domain.Common.Errors;

public static class VariantImageErrors
{
    public static readonly Error SortOrderInvalid = new(ErrorCodes.VariantImage.SortOrderInvalid, "Variant image sort order is invalid.", ErrorType.Validation);
    public static readonly Error UrlEmpty = new(ErrorCodes.VariantImage.UrlEmpty, "Variant image URL is empty.", ErrorType.Validation);
    public static readonly Error VariantIdEmpty = new(ErrorCodes.VariantImage.VariantIdEmpty, "Variant image variant ID is empty.", ErrorType.Validation);
    public static readonly Error StorageKeyEmpty = new(ErrorCodes.VariantImage.StorageKeyEmpty, "Variant image storage key is empty.", ErrorType.Validation);
    public static readonly Error WidthInvalid = new(ErrorCodes.VariantImage.WidthInvalid, "Variant image width is invalid.", ErrorType.Validation);
    public static readonly Error HeightInvalid = new(ErrorCodes.VariantImage.HeightInvalid, "Variant image height is invalid.", ErrorType.Validation);
    public static readonly Error SizeInvalid = new(ErrorCodes.VariantImage.SizeInvalid, "Variant image size is invalid.", ErrorType.Validation);
    public static readonly Error FormatEmpty = new(ErrorCodes.VariantImage.FormatEmpty, "Variant image format is empty.", ErrorType.Validation);
    public static readonly Error InvalidProcessingStatus = new(ErrorCodes.VariantImage.InvalidProcessingStatus, "Variant image processing status is invalid.", ErrorType.Validation);
    public static readonly Error ImageRequired = new(ErrorCodes.VariantImage.ImageRequired, "Variant image is required.", ErrorType.Validation);
    public static readonly Error ImageNotFound = new(ErrorCodes.VariantImage.ImageNotFound, "Variant image was not found.", ErrorType.NotFound);
}
