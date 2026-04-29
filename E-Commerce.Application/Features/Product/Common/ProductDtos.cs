using E_Commerce.Application.Common.Dtos;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Product.Common;

public sealed record ProductListItemDto(
    Guid Id,
    Guid CategoryId,
    string CategorySlug,
    string Slug,
    string? Brand,
    string Status,
    MoneyDto BasePrice,
    bool IsActive,
    int VariantCount);

public sealed record ProductVariantDto(
    Guid Id,
    string Sku,
    string? Size,
    string? Color,
    MoneyDto? PriceOverride,
    bool IsActive);

public sealed record ProductDetailDto(
    Guid Id,
    Guid CategoryId,
    string CategorySlug,
    string Slug,
    string? Brand,
    string Status,
    MoneyDto BasePrice,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<ProductVariantDto> Variants);

internal static class ProductDtoMappings
{
    public static ProductListItemDto ToListItemDto(this E_Commerce.Domain.Entities.Product product)
        => new(
            product.Id,
            product.CategoryId,
            product.Category.Slug.Value,
            product.Slug.Value,
            product.Brand,
            product.Status.ToString(),
            MoneyDto.FromMoney(product.BasePrice),
            product.IsActive,
            product.Variants.Count);

    public static ProductDetailDto ToDetailDto(this E_Commerce.Domain.Entities.Product product)
        => new(
            product.Id,
            product.CategoryId,
            product.Category.Slug.Value,
            product.Slug.Value,
            product.Brand,
            product.Status.ToString(),
            MoneyDto.FromMoney(product.BasePrice),
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt,
            product.Variants
                .OrderBy(x => x.Sku)
                .Select(x => new ProductVariantDto(
                    x.Id,
                    x.Sku,
                    x.Size,
                    x.Color,
                    x.PriceOverride is null ? null : MoneyDto.FromMoney(x.PriceOverride),
                    x.IsActive))
                .ToArray());
}



