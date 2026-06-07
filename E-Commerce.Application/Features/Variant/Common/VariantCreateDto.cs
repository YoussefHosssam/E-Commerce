using E_Commerce.Application.Common.Dtos;

namespace E_Commerce.Application.Features.Variant.Common;

public sealed record VariantCreateDto(
    string Sku,
    string? Size,
    ColorDto? Color,
    decimal? VariantPriceOverrideAmount,
    int Stock,
    bool IsDefault,
    bool IsActive = true);
