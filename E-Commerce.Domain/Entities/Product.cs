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
        Money basePrice)
    {
        CategoryId = categoryId;
        Slug = slug;
        Status = status;
        Brand = brand;
        BasePrice = basePrice;
        Name = name;
        IsActive = true;
        UpdatedAt = null;
    }

    public static Product Create(
        string name,
        Guid categoryId,
        Slug slug,
        Money basePrice,
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

        return new Product(name , categoryId, slug, status, brand, basePrice);
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

        _images.Add(image);
        Touch(now);
    }

    // ? override ????? ???? Money? ?? decimal?
    // ? ???? ???? ?? override currency ??? BasePrice currency (????? ????)
    public Variant AddVariant(
        string sku,
        string? size,
        string? color,
        Money? priceOverride,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainValidationException(ProductErrors.VariantSkuRequired);

        var normalizedSku = sku.Trim().ToUpperInvariant();

        if (_variants.Any(v => v.Sku == normalizedSku))
            throw new DomainValidationException(ProductErrors.VariantSkuDuplicate);

        if (priceOverride is not null)
        {
            if (priceOverride.Amount < 0)
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideInvalid);

            // important: ?????? variant override ????? ?????? ?? ??????
            if (!Equals(priceOverride.Currency, BasePrice.Currency))
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideCurrencyMismatch);
        }

        // ?? ???? ????? Variant.Create signature ???? ?????? Money? ??? decimal?
        var variant = Variant.Create(this.Id, normalizedSku, size, color, priceOverride);

        _variants.Add(variant);
        Touch(now);
        return variant;
    }

    public Variant UpdateVariant(
        Guid variantId,
        string sku,
        string? size,
        string? color,
        Money? priceOverride,
        bool isActive,
        DateTimeOffset now)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new DomainValidationException(ProductErrors.VariantNotFound);

        var normalizedSku = sku.Trim().ToUpperInvariant();

        if (_variants.Any(v => v.Id != variantId && v.Sku == normalizedSku))
            throw new DomainValidationException(ProductErrors.VariantSkuDuplicate);

        if (priceOverride is not null)
        {
            if (priceOverride.Amount < 0)
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideInvalid);

            if (!Equals(priceOverride.Currency, BasePrice.Currency))
                throw new DomainValidationException(ProductErrors.VariantPriceOverrideCurrencyMismatch);
        }

        variant.ChangeSku(normalizedSku);
        variant.ChangeAttributes(size, color);
        variant.SetPriceOverride(priceOverride);

        if (isActive)
            variant.Activate();
        else
            variant.Deactivate();

        Touch(now);
        return variant;
    }

    public void RemoveVariant(Guid variantId, DateTimeOffset now)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new DomainValidationException(ProductErrors.VariantNotFound);

        _variants.Remove(variant);
        Touch(now);
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

