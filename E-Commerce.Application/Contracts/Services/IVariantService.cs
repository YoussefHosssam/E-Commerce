using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Services;

public interface IVariantService
{
    Task<Result<IReadOnlyList<Variant>>> CreateVariantsForProductAsync(
        Product product,
        IReadOnlyCollection<VariantCreateDto> variants,
        CancellationToken cancellationToken);
}
