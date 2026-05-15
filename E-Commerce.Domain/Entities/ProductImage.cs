using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;

namespace E_Commerce.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public string StorageKey { get; private set; } = default!;
    public string Url { get; private set; } = string.Empty;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public long SizeInBytes { get; private set; }
    public string Format { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }
    public ImageProcessingStatus ProcessingStatus { get; private set; } = ImageProcessingStatus.PendingUpload;

    private ProductImage() { }

    private ProductImage(Guid productId, string storageKey, bool isPrimary, int sortOrder)
    {
        ProductId = productId;
        StorageKey = storageKey;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
        ProcessingStatus = ImageProcessingStatus.PendingUpload;
    }

    public static ProductImage CreatePending(Guid productId, string storageKey, bool isPrimary, int sortOrder)
    {
        if (productId == Guid.Empty)
            throw new DomainValidationException(ProductImageErrors.ProductIdEmpty);

        storageKey = NormalizeRequired(storageKey, ProductImageErrors.StorageKeyEmpty);

        if (sortOrder <= 0)
            throw new DomainValidationException(ProductImageErrors.SortOrderInvalid);

        return new ProductImage(productId, storageKey, isPrimary, sortOrder);
    }

    public void MarkUploaded(string url, int width, int height, long sizeInBytes, string format)
    {
        SetMetadata(url, width, height, sizeInBytes, format);
        ProcessingStatus = ImageProcessingStatus.Uploaded;
    }

    public void MarkReady(string url, int width, int height, long sizeInBytes, string format)
    {
        SetMetadata(url, width, height, sizeInBytes, format);
        ProcessingStatus = ImageProcessingStatus.Ready;
    }

    public void MarkFailed()
    {
        ProcessingStatus = ImageProcessingStatus.Failed;
    }

    public void MarkDeleted()
    {
        ProcessingStatus = ImageProcessingStatus.Deleted;
        IsPrimary = false;
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void ChangeSortOrder(int sortOrder)
    {
        if (sortOrder <= 0)
            throw new DomainValidationException(ProductImageErrors.SortOrderInvalid);

        SortOrder = sortOrder;
    }

    private void SetMetadata(string url, int width, int height, long sizeInBytes, string format)
    {
        Url = NormalizeRequired(url, ProductImageErrors.UrlEmpty);
        Format = NormalizeRequired(format, ProductImageErrors.FormatEmpty).ToLowerInvariant();

        if (width <= 0)
            throw new DomainValidationException(ProductImageErrors.WidthInvalid);

        if (height <= 0)
            throw new DomainValidationException(ProductImageErrors.HeightInvalid);

        if (sizeInBytes <= 0)
            throw new DomainValidationException(ProductImageErrors.SizeInvalid);

        Width = width;
        Height = height;
        SizeInBytes = sizeInBytes;
    }

    private static string NormalizeRequired(string value, Error error)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(error);

        return value.Trim();
    }
}
