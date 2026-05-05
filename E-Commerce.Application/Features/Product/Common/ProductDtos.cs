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
