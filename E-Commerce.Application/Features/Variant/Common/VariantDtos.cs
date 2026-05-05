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
