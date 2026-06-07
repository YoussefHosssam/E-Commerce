using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Variant.Common;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant;

public sealed record UpdateVariantCommand(
    Guid ProductId,
    Guid VariantId,
    string Sku,
    string? Size,
    ColorDto? Color,
    bool? HasPriceOverride,
    decimal? VariantPriceOverrideAmount,
    bool IsDefault,
    bool IsActive) : IRequest<Result<VariantDetailDto>>;

