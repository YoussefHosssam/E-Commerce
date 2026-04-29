using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Variant.Common;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed record CreateVariantCommand(
    Guid ProductId,
    string Sku,
    string? Size,
    string? Color,
    decimal? PriceOverrideAmount,
    string? PriceOverrideCurrency,
    bool IsActive) : IRequest<Result<VariantDetailDto>>;

