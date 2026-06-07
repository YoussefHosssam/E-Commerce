using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Product : BaseEntity
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!; // EF navigation
    public string Name { get; private set; } = default!;
    public Slug Slug { get; private set; } = default!; // unique (enforced via DB too)
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;

    public string? Brand { get; private set; }

    // ? Money ??? (Currency + decimal)
    public Money BasePrice { get; private set; } = default!;
    public bool HasVariants { get; private set; }
    public bool HasDiscount { get; private set; }
    public Money? CompareAtPrice { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private readonly List<Variant> _variants = new();
    public IReadOnlyCollection<Variant> Variants => _variants.AsReadOnly();

    private readonly List<Favorite> _favorites = new();
    public IReadOnlyCollection<Favorite> Favorites => _favorites.AsReadOnly();

    private Product() { } // EF

    private Product(
        string name,
        Guid categoryId,
        Slug slug,
        ProductStatus status,
        string? brand,
        Money basePrice,
        bool hasVariants)
    {
        CategoryId = categoryId;
        Slug = slug;
        Status = status;
        Brand = brand;
        BasePrice = basePrice;
        HasVariants = hasVariants;
        HasDiscount = false;
        CompareAtPrice = null;
        Name = name;
        IsActive = true;
        UpdatedAt = null;
    }

    public static Product Create(
        string name,
        Guid categoryId,
        Slug slug,
        Money basePrice,
        bool hasVariants,
        string? brand = null,
        ProductStatus status = ProductStatus.Draft)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new DomainValidationException(ProductErrors.NameInvalid);

        if (categoryId == Guid.Empty)
            throw new DomainValidationException(ProductErrors.CategoryRequired);

        if (slug.Equals(default(Slug)))
            throw new DomainValidationException(ProductErrors.SlugRequired);

        if (!Enum.IsDefined(typeof(ProductStatus), status))
            throw new DomainValidationException(ProductErrors.StatusInvalid);

        if (basePrice is null)
            throw new DomainValidationException(ProductErrors.BasePriceRequired);

        if (basePrice.Amount < 0)
            throw new DomainValidationException(ProductErrors.BasePriceInvalid);

        if (string.IsNullOrWhiteSpace(basePrice.Currency.Value))
            throw new DomainValidationException(ProductErrors.CurrencyRequired);

        brand = NormalizeBrandOrNull(brand);

        return new Product(name , categoryId, slug, status, brand, basePrice, hasVariants);
    }

    // -------- Domain behaviors --------

    public void ChangeBasePrice(Money newBasePrice, DateTimeOffset now)
    {
        if (newBasePrice is null)
            throw new DomainValidationException(ProductErrors.BasePriceRequired);

        if (newBasePrice.Amount < 0)
            throw new DomainValidationException(ProductErrors.BasePriceInvalid);

        if (string.IsNullOrWhiteSpace(newBasePrice.Currency.Value))
            throw new DomainValidationException(ProductErrors.CurrencyRequired);

        BasePrice = newBasePrice;
        Touch(now);
    }

    public void ChangeVariantMode(bool hasVariants, DateTimeOffset now)
    {
        HasVariants = hasVariants;
        Touch(now);
    }

    public void ApplyDiscount(Money compareAtPrice)
    {
        if (compareAtPrice is null || compareAtPrice.Amount <= 0)
            throw new DomainValidationException(ProductErrors.DiscountPriceRequired);

        var actualPrice = GetActualSellingPrice();
        if (compareAtPrice.Currency != actualPrice.Currency || compareAtPrice.Amount <= actualPrice.Amount)
            throw new DomainValidationException(ProductErrors.DiscountPriceMustBeGreaterThanActualPrice);

        CompareAtPrice = compareAtPrice;
        HasDiscount = true;
    }

    public void RemoveDiscount()
    {
        CompareAtPrice = null;
        HasDiscount = false;
    }

    public void ChangeCategory(Guid categoryId, DateTimeOffset now)
    {
        if (categoryId == Guid.Empty)
            throw new DomainValidationException(ProductErrors.CategoryRequired);

        CategoryId = categoryId;
        Touch(now);
    }

    public void ChangeSlug(Slug slug, DateTimeOffset now)
    {
        if (slug.Equals(default(Slug)))
            throw new DomainValidationException(ProductErrors.SlugRequired);

        Slug = slug;
        Touch(now);
    }

    public void ChangeStatus(ProductStatus status, DateTimeOffset now)
    {
        if (!Enum.IsDefined(typeof(ProductStatus), status))
            throw new DomainValidationException(ProductErrors.StatusInvalid);

        Status = status;
        Touch(now);
    }
    public void ChangeName(string name, DateTimeOffset now)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new DomainValidationException(ProductErrors.NameInvalid);

        Name = name;
        Touch(now);
    }
    
    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        Touch(now);
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        Touch(now);
    }

    public void ChangeBrand(string? brand, DateTimeOffset now)
    {
        Brand = NormalizeBrandOrNull(brand);
        Touch(now);
    }

    public void AddImage(ProductImage image, DateTimeOffset now)
    {
        if (image is null)
            throw new DomainValidationException(ProductErrors.ImageRequired);

        if (image.ProductId != Id)
            throw new DomainValidationException(ProductImageErrors.ProductIdEmpty);

        if (!_images.Any(x => x.ProcessingStatus != ImageProcessingStatus.Deleted))
            image.SetPrimary(true);

        if (image.IsPrimary)
            UnsetPrimaryImages();

        _images.Add(image);
        Touch(now);
    }

    public ProductImage GetImage(Guid imageId)
    {
        return _images.FirstOrDefault(x => x.Id == imageId && x.ProcessingStatus != ImageProcessingStatus.Deleted)
            ?? throw new DomainValidationException(ProductImageErrors.ImageNotFound);
    }

    public ProductImage GetImageByStorageKey(string storageKey)
    {
        return _images.FirstOrDefault(x => x.StorageKey == storageKey && x.ProcessingStatus != ImageProcessingStatus.Deleted)
            ?? throw new DomainValidationException(ProductImageErrors.ImageNotFound);
    }

    public void SetPrimaryImage(Guid imageId, DateTimeOffset now)
    {
        var image = GetImage(imageId);

        UnsetPrimaryImages();
        image.SetPrimary(true);
        Touch(now);
    }

    public void DeleteImage(Guid imageId, DateTimeOffset now)
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

        Touch(now);
    }

    public void ReorderImages(IReadOnlyDictionary<Guid, int> sortOrders, DateTimeOffset now)
    {
        foreach (var (imageId, sortOrder) in sortOrders)
        {
            GetImage(imageId).ChangeSortOrder(sortOrder);
        }

        Touch(now);
    }

    private void UnsetPrimaryImages()
    {
        foreach (var existing in _images)
        {
            existing.SetPrimary(false);
        }
    }

    // ? override ????? ???? Money? ?? decimal?
    // ? ???? ???? ?? override currency ??? BasePrice currency (????? ????)
    public Variant AddVariant(
        string sku,
        string? size,
        Color color,
        Money? price,
        bool isDefault,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainValidationException(ProductErrors.VariantSkuRequired);

        var normalizedSku = sku.Trim().ToUpperInvariant();

        if (_variants.Any(v => v.Sku == normalizedSku))
            throw new DomainValidationException(ProductErrors.VariantSkuDuplicate);

        if (price is not null)
        {
            if (price.Amount <= 0)
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideInvalid);

            if (!Equals(price.Currency, BasePrice.Currency))
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideCurrencyMismatch);
        }

        if (isDefault)
            ClearDefaultVariant();

        var variant = Variant.Create(this.Id, normalizedSku, size, color, price, isDefault, BasePrice.Currency);

        _variants.Add(variant);
        Touch(now);
        return variant;
    }

    public Variant UpdateVariant(
        Guid variantId,
        string sku,
        string? size,
        Color color,
        Money? price,
        bool updatePrice,
        bool isDefault,
        bool isActive,
        DateTimeOffset now)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new DomainValidationException(ProductErrors.VariantNotFound);

        var normalizedSku = sku.Trim().ToUpperInvariant();

        if (_variants.Any(v => v.Id != variantId && v.Sku == normalizedSku))
            throw new DomainValidationException(ProductErrors.VariantSkuDuplicate);

        if (updatePrice && price is not null)
        {
            if (price.Amount <= 0)
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideInvalid);

            if (!Equals(price.Currency, BasePrice.Currency))
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideCurrencyMismatch);
        }

        variant.ChangeSku(normalizedSku);
        variant.ChangeAttributes(size, color);
        if (updatePrice)
            variant.ChangePrice(price, BasePrice.Currency);

        if (isActive)
            variant.Activate();
        else
            variant.Deactivate();

        if (isDefault)
        {
            ClearDefaultVariant(exceptVariantId: variantId);
            variant.SetDefault();
        }
        else if (variant.IsDefault && !_variants.Any(v => v.Id != variantId && v.IsActive && v.IsDefault))
        {
            throw new DomainValidationException(ProductErrors.ProductMustHaveOneDefaultVariant);
        }
        else
        {
            variant.UnsetDefault();
        }

        Touch(now);
        return variant;
    }

    public void RemoveVariant(Guid variantId, DateTimeOffset now)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new DomainValidationException(ProductErrors.VariantNotFound);

        if (_variants.Count(v => v.IsActive) <= 1)
            throw new DomainValidationException(VariantErrors.CannotDeleteLastActiveVariant);

        if (variant.IsDefault && !_variants.Any(v => v.Id != variantId && v.IsActive))
            throw new DomainValidationException(ProductErrors.ProductMustHaveOneDefaultVariant);

        _variants.Remove(variant);

        if (variant.IsDefault)
            _variants.First(v => v.IsActive).SetDefault();

        Touch(now);
    }

    public void ArchiveVariant(Guid variantId, DateTimeOffset now)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new DomainValidationException(ProductErrors.VariantNotFound);

        if (_variants.Count(v => v.IsActive) <= 1)
            throw new DomainValidationException(VariantErrors.CannotDeleteLastActiveVariant);

        variant.Deactivate();

        if (variant.IsDefault)
        {
            variant.UnsetDefault();
            _variants.First(v => v.IsActive).SetDefault();
        }

        Touch(now);
    }

    public void ArchiveActiveVariantsForReplacement(DateTimeOffset now)
    {
        foreach (var variant in _variants.Where(v => v.IsActive))
        {
            variant.Deactivate();
            variant.UnsetDefault();
        }

        Touch(now);
    }

    private Money GetActualSellingPrice()
    {
        var activeVariants = _variants.Where(v => v.IsActive).ToList();
        if (activeVariants.Count == 0)
            throw new DomainValidationException(ProductErrors.ProductMustHaveAtLeastOneVariant);

        return activeVariants.FirstOrDefault(v => v.IsDefault)?.GetEffectivePrice(BasePrice)
            ?? activeVariants.Select(v => v.GetEffectivePrice(BasePrice)).OrderBy(m => m.Amount).First();
    }

    private void ClearDefaultVariant(Guid? exceptVariantId = null)
    {
        foreach (var variant in _variants.Where(v => v.Id != exceptVariantId))
        {
            variant.UnsetDefault();
        }
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(ProductErrors.NowRequired);

        UpdatedAt = now;
    }

    private static string? NormalizeBrandOrNull(string? brand)
    {
        if (brand is null) return null;

        brand = brand.Trim();
        if (brand.Length == 0) return null;

        if (brand.Length > 80)
            throw new DomainValidationException(ProductErrors.BrandTooLong);

        return brand;
    }
}

