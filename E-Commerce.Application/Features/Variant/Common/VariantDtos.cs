using E_Commerce.Application.Common.Dtos;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Variant.Common;

public sealed record VariantListItemDto(
    Guid Id,
    Guid ProductId,
    string ProductSlug,
    string Sku,
    string? Size,
    string? Color,
    MoneyDto? PriceOverride,
    bool IsActive);
public sealed record CartVariantDto(
    Guid Id ,
    string Sku,
    string? Size,
    string? Color,
    MoneyDto? PriceOverride);
public sealed record VariantDetailDto(
    Guid Id,
    Guid ProductId,
    string ProductSlug,
    string Sku,
    string? Size,
    string? Color,
    MoneyDto? PriceOverride,
    bool IsActive);

internal static class VariantDtoMappings
{
    public static VariantListItemDto ToListItemDto(this E_Commerce.Domain.Entities.Variant variant)
        => new(
            variant.Id,
            variant.ProductId,
            variant.Product.Slug.Value,
            variant.Sku,
            variant.Size,
            variant.Color,
            variant.PriceOverride is null ? null : MoneyDto.FromMoney(variant.PriceOverride),
            variant.IsActive);

    public static VariantDetailDto ToDetailDto(this E_Commerce.Domain.Entities.Variant variant)
        => new(
            variant.Id,
            variant.ProductId,
            variant.Product.Slug.Value,
            variant.Sku,
            variant.Size,
            variant.Color,
            variant.PriceOverride is null ? null : MoneyDto.FromMoney(variant.PriceOverride),
            variant.IsActive);
    public static CartVariantDto ToCartVariantDto(this E_Commerce.Domain.Entities.Variant variant) 
        => new(
            variant.Id,
            variant.Sku,
            variant.Size,
            variant.Color,
            variant.PriceOverride is null ? null : MoneyDto.FromMoney(variant.PriceOverride)
           );
}



