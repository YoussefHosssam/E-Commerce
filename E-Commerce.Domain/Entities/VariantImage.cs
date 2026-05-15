using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;

namespace E_Commerce.Domain.Entities;

public class VariantImage : BaseEntity
{
    public Guid VariantId { get; private set; }
    public string StorageKey { get; private set; } = default!;
    public string Url { get; private set; } = string.Empty;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public long SizeInBytes { get; private set; }
    public string Format { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }
    public ImageProcessingStatus ProcessingStatus { get; private set; } = ImageProcessingStatus.PendingUpload;

    private VariantImage() { }

    private VariantImage(Guid variantId, string storageKey, bool isPrimary, int sortOrder)
    {
        VariantId = variantId;
        StorageKey = storageKey;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
        ProcessingStatus = ImageProcessingStatus.PendingUpload;
    }

    public static VariantImage CreatePending(Guid variantId, string storageKey, bool isPrimary, int sortOrder)
    {
        if (variantId == Guid.Empty)
            throw new DomainValidationException(VariantImageErrors.VariantIdEmpty);

        storageKey = NormalizeRequired(storageKey, VariantImageErrors.StorageKeyEmpty);

        if (sortOrder <= 0)
            throw new DomainValidationException(VariantImageErrors.SortOrderInvalid);

        return new VariantImage(variantId, storageKey, isPrimary, sortOrder);
    }

    public static VariantImage Create(Guid variantId, string url, bool isPrimary, int sortOrder)
    {
        var image = CreatePending(variantId, url, isPrimary, sortOrder);
        image.MarkReady(url, 1, 1, 1, "unknown");
        return image;
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
            throw new DomainValidationException(VariantImageErrors.SortOrderInvalid);

        SortOrder = sortOrder;
    }

    private void SetMetadata(string url, int width, int height, long sizeInBytes, string format)
    {
        Url = NormalizeRequired(url, VariantImageErrors.UrlEmpty);
        Format = NormalizeRequired(format, VariantImageErrors.FormatEmpty).ToLowerInvariant();

        if (width <= 0)
            throw new DomainValidationException(VariantImageErrors.WidthInvalid);

        if (height <= 0)
            throw new DomainValidationException(VariantImageErrors.HeightInvalid);

        if (sizeInBytes <= 0)
            throw new DomainValidationException(VariantImageErrors.SizeInvalid);

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
