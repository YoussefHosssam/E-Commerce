using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Variant : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!; // EF navigation

    public string Sku { get; private set; } = default!; // unique (DB unique index too)
    public string? Size { get; private set; }
    public string? Color { get; private set; }

    // ? Money override ??? decimal
    public Money? PriceOverride { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<VariantImage> _images = new();
    public IReadOnlyCollection<VariantImage> Images => _images.AsReadOnly();

    public Inventory? Inventory { get; private set; }

    private Variant() { } // EF

    private Variant(
        Guid productId,
        string sku,
        string? size,
        string? color,
        Money? priceOverride)
    {
        ProductId = productId;
        Sku = sku;
        Size = size;
        Color = color;
        PriceOverride = priceOverride;

        IsActive = true;
    }

    public static Variant Create(
        Guid productId,
        string sku,
        string? size,
        string? color,
        Money? priceOverride)
    {
        if (productId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Variant.ProductRequired);

        sku = NormalizeSku(sku);

        size = NormalizeOptional(size, 30, ErrorCodes.Domain.Variant.SizeTooLong, "Size is too long.");
        color = NormalizeOptional(color, 30, ErrorCodes.Domain.Variant.ColorTooLong, "Color is too long.");

        ValidatePriceOverride(priceOverride);

        return new Variant(productId, sku, size, color, priceOverride);
    }

    public void ChangeSku(string sku)
    {
        // ??????: uniqueness ??????? ????? ?? DB/Application (query)
        Sku = NormalizeSku(sku);
    }

    public void ChangeAttributes(string? size, string? color)
    {
        Size = NormalizeOptional(size, 30, ErrorCodes.Domain.Variant.SizeTooLong, "Size is too long.");
        Color = NormalizeOptional(color, 30, ErrorCodes.Domain.Variant.ColorTooLong, "Color is too long.");
    }

    public void SetPriceOverride(Money? priceOverride)
    {
        ValidatePriceOverride(priceOverride);
        PriceOverride = priceOverride;
    }

    public void ClearPriceOverride() => PriceOverride = null;

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void AddImage(VariantImage image)
    {
        if (image is null)
            throw new DomainValidationException(ErrorCodes.Domain.Variant.ImageRequired);

        _images.Add(image);
    }

    public void SetInventory(Inventory inventory)
    {
        Inventory = inventory ?? throw new DomainValidationException(ErrorCodes.Domain.Variant.InventoryRequired);
    }

    private static void ValidatePriceOverride(Money? priceOverride)
    {
        if (priceOverride is null) return;

        if (priceOverride.Amount < 0)
            throw new DomainValidationException(ErrorCodes.Variant.PriceInvalid);

        if (string.IsNullOrWhiteSpace(priceOverride.Currency.Value))
            throw new DomainValidationException(ErrorCodes.Variant.CurrencyRequired);
    }

    private static string NormalizeSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainValidationException(ErrorCodes.Variant.SkuRequired);

        sku = sku.Trim().ToUpperInvariant();

        if (sku.Length > 64)
            throw new DomainValidationException(ErrorCodes.Variant.SkuTooLong);

        return sku;
    }

    private static string? NormalizeOptional(string? value, int maxLen, string code, string message)
    {
        if (value is null) return null;

        value = value.Trim();
        if (value.Length == 0) return null;

        if (value.Length > maxLen)
            throw new DomainValidationException(code);

        return value;
    }
}


