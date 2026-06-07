using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Variant : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!; // EF navigation

    public string Sku { get; private set; } = default!; // unique (DB unique index too)
    public string? Size { get; private set; }
    public Color Color { get; private set; } = default!;
    public Money? Price { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsDefault { get; private set; }

    private readonly List<VariantImage> _images = new();
    public IReadOnlyCollection<VariantImage> Images => _images.AsReadOnly();

    public Inventory? Inventory { get; private set; }

    private Variant() { } // EF

    private Variant(
        Guid productId,
        string sku,
        string? size,
        Color color,
        Money? price)
    {
        ProductId = productId;
        Sku = sku;
        Size = size;
        Color = color;
        Price = price;

        IsActive = true;
    }

    public static Variant Create(
        Guid productId,
        string sku,
        string? size,
        Color color,
        Money? price,
        bool isDefault,
        CurrencyCode productCurrency)
    {
        if (productId == Guid.Empty)
            throw new DomainValidationException(VariantErrors.ProductRequired);

        sku = NormalizeSku(sku);

        size = NormalizeOptional(size, 30, VariantErrors.SizeTooLong);

        if (color is null)
            throw new DomainValidationException(VariantErrors.ColorRequired);

        ValidatePrice(price, productCurrency);

        var variant = new Variant(productId, sku, size, color, price);
        if (isDefault)
            variant.SetDefault();

        return variant;
    }

    public void ChangeSku(string sku)
    {
        Sku = NormalizeSku(sku);
    }

    public void ChangeAttributes(string? size, Color color)
    {
        Size = NormalizeOptional(size, 30, VariantErrors.SizeTooLong);
        ChangeColor(color);
    }

    public void ChangeColor(Color color)
    {
        Color = color ?? throw new DomainValidationException(VariantErrors.ColorRequired);
    }

    public void ChangePrice(Money? price, CurrencyCode productCurrency)
    {
        ValidatePrice(price, productCurrency);
        Price = price;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;

    public void AddImage(VariantImage image)
    {
        if (image is null)
            throw new DomainValidationException(VariantErrors.ImageRequired);

        if (image.VariantId != Id)
            throw new DomainValidationException(VariantImageErrors.VariantIdEmpty);

        if (!_images.Any(x => x.ProcessingStatus != ImageProcessingStatus.Deleted))
            image.SetPrimary(true);

        if (image.IsPrimary)
            UnsetPrimaryImages();

        _images.Add(image);
    }

    public string? GetPrimaryImage()
    {
        var img = _images.FirstOrDefault(i => i.IsPrimary);
        return img == null ? null : img.Url;
    }
    public VariantImage GetImage(Guid imageId)
    {
        return _images.FirstOrDefault(x => x.Id == imageId && x.ProcessingStatus != ImageProcessingStatus.Deleted)
            ?? throw new DomainValidationException(VariantImageErrors.ImageNotFound);
    }

    public VariantImage GetImageByStorageKey(string storageKey)
    {
        return _images.FirstOrDefault(x => x.StorageKey == storageKey && x.ProcessingStatus != ImageProcessingStatus.Deleted)
            ?? throw new DomainValidationException(VariantImageErrors.ImageNotFound);
    }

    public void SetPrimaryImage(Guid imageId)
    {
        var image = GetImage(imageId);

        UnsetPrimaryImages();
        image.SetPrimary(true);
    }

    public void DeleteImage(Guid imageId)
    {
        var image = GetImage(imageId);
        var wasPrimary = image.IsPrimary;

        image.MarkDeleted();

        if (wasPrimary)
        {
            var next = _images
                .Where(x => x.ProcessingStatus != ImageProcessingStatus.Deleted && x.ProcessingStatus == ImageProcessingStatus.Uploaded)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefault();

            next?.SetPrimary(true);
        }
    }

    public void ReorderImages(IReadOnlyDictionary<Guid, int> sortOrders)
    {
        foreach (var (imageId, sortOrder) in sortOrders)
        {
            GetImage(imageId).ChangeSortOrder(sortOrder);
        }
    }

    private void UnsetPrimaryImages()
    {
        foreach (var existing in _images)
        {
            existing.SetPrimary(false);
        }
    }

    public void SetInventory(Inventory inventory)
    {
        Inventory = inventory ?? throw new DomainValidationException(VariantErrors.InventoryRequired);
    }
    public Money GetPrice()
    {
        return GetEffectivePrice(Product.BasePrice);
    }

    public Money GetEffectivePrice(Money productPrice)
    {
        if (productPrice is null)
            throw new DomainValidationException(ProductErrors.BasePriceRequired);

        return Price ?? productPrice;
    }

    private static void ValidatePrice(Money? price, CurrencyCode productCurrency)
    {
        if (price is null) return;

        if (price.Amount <= 0)
            throw new DomainValidationException(VariantErrors.PriceInvalid);

        if (string.IsNullOrWhiteSpace(price.Currency.Value))
            throw new DomainValidationException(VariantErrors.CurrencyRequired);

        if (price.Currency != productCurrency)
            throw new DomainValidationException(VariantErrors.CurrencyInvalid);
    }

    private static string NormalizeSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainValidationException(VariantErrors.SkuRequired);

        sku = sku.Trim().ToUpperInvariant();

        if (sku.Length > 64)
            throw new DomainValidationException(VariantErrors.SkuTooLong);

        return sku;
    }

    private static string? NormalizeOptional(string? value, int maxLen,Error error)
    {
        if (value is null) return null;

        value = value.Trim();
        if (value.Length == 0) return null;

        if (value.Length > maxLen)
            throw new DomainValidationException(error);

        return value;
    }
}


