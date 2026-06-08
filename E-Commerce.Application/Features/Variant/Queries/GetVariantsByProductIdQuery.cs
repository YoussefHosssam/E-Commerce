using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Queries;

public sealed record GetVariantsByProductIdQuery(Guid ProductId) : IRequest<Result<IReadOnlyCollection<VariantListItemDto>>>;

public sealed class GetVariantsByProductIdHandler : IRequestHandler<GetVariantsByProductIdQuery, Result<IReadOnlyCollection<VariantListItemDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetVariantsByProductIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IReadOnlyCollection<VariantListItemDto>>> Handle(GetVariantsByProductIdQuery request, CancellationToken cancellationToken)
    {
        var items = await _uow.Variants.GetVariantListItemDtosByProductIdAsync(request.ProductId, cancellationToken);
        if (items is null)
        {
            return Result<IReadOnlyCollection<VariantListItemDto>>.Fail(ProductErrors.NotFound);
        }

        return Result<IReadOnlyCollection<VariantListItemDto>>.Success(items);
    }
}
