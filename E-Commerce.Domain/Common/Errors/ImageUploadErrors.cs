namespace E_Commerce.Domain.Common.Errors;

public static class ImageUploadErrors
{
    public static readonly Error ContentTypeInvalid = new(ErrorCodes.ImageUpload.ContentTypeInvalid, "Image content type is not allowed.", ErrorType.Validation);
    public static readonly Error SizeInvalid = new(ErrorCodes.ImageUpload.SizeInvalid, "Image size is invalid.", ErrorType.Validation);
    public static readonly Error FormatInvalid = new(ErrorCodes.ImageUpload.FormatInvalid, "Image format is not allowed.", ErrorType.Validation);
    public static readonly Error DimensionsInvalid = new(ErrorCodes.ImageUpload.DimensionsInvalid, "Image dimensions are invalid.", ErrorType.Validation);
    public static readonly Error StorageKeyInvalid = new(ErrorCodes.ImageUpload.StorageKeyInvalid, "Image storage key is invalid.", ErrorType.Validation);
    public static readonly Error UploadedResourceNotFound = new(ErrorCodes.ImageUpload.UploadedResourceNotFound, "Uploaded image resource was not found.", ErrorType.NotFound);
}
