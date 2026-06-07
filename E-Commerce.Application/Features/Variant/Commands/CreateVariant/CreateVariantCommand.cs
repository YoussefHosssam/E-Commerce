using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Variant.Common;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed record CreateVariantCommand(
    Guid ProductId,
    IReadOnlyCollection<VariantCreateDto> Variants) : IRequest<Result<IReadOnlyCollection<VariantDetailDto>>>;

