using E_Commerce.Application.Common.Options;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using Microsoft.Extensions.Options;

namespace E_Commerce.Application.Features.ImageUploads.Common;

internal sealed class ImageUploadValidationService : IImageUploadValidationService
{
    private readonly ImageStorageOptions _options;

    public ImageUploadValidationService(IOptions<ImageStorageOptions> options)
    {
        _options = options.Value;
    }

    public Error? ValidateRequestedFile(string contentType, long sizeInBytes)
    {
        if (sizeInBytes <= 0 || sizeInBytes > _options.MaxImageBytes)
            return ImageUploadErrors.SizeInvalid;

        if (!_options.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            return ImageUploadErrors.ContentTypeInvalid;

        return null;
    }

    public Error? ValidateVerifiedResource(ImageUploadVerificationResult resource)
    {
        if (resource.SizeInBytes <= 0 || resource.SizeInBytes > _options.MaxImageBytes)
            return ImageUploadErrors.SizeInvalid;

        if (!_options.AllowedFormats.Contains(resource.Format, StringComparer.OrdinalIgnoreCase))
            return ImageUploadErrors.FormatInvalid;

        if (resource.Width < _options.MinWidth ||
            resource.Height < _options.MinHeight ||
            resource.Width > _options.MaxWidth ||
            resource.Height > _options.MaxHeight)
        {
            return ImageUploadErrors.DimensionsInvalid;
        }

        return null;
    }

    public bool HasExpectedPrefix<T>(string storageKey , Guid id)
    {

        return storageKey.StartsWith(GetExpectedPrefix<T>(id), StringComparison.Ordinal);
    }

    private string GetExpectedPrefix<T>(Guid id)
    {
        if (typeof(T) == typeof(VariantImage))
            return $"{_options.UploadFolderRoot.TrimEnd('/')}/variants/{id:N}/images/";
        if (typeof(T) == typeof(ProductImage))
            return $"{_options.UploadFolderRoot.TrimEnd('/')}/products/{id:N}/images/";
        else
            return "";
    }
}
