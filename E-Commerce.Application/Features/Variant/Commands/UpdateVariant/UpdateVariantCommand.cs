using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Variant.Common;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant;

public sealed record UpdateVariantCommand(
    Guid ProductId,
    Guid VariantId,
    string Sku,
    string? Size,
    string? Color,
    decimal? PriceOverrideAmount,
    string? PriceOverrideCurrency,
    bool IsActive) : IRequest<Result<VariantDetailDto>>;

