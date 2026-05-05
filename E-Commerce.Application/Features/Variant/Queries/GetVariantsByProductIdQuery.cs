using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Variant.Queries;

public sealed record GetVariantsByProductIdQuery(Guid ProductId) : IRequest<Result<IReadOnlyCollection<VariantListItemDto>>>;

public sealed class GetVariantsByProductIdHandler : IRequestHandler<GetVariantsByProductIdQuery, Result<IReadOnlyCollection<VariantListItemDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetVariantsByProductIdHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<VariantListItemDto>>> Handle(GetVariantsByProductIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, false, cancellationToken);
        if (product is null)
        {
            return Result<IReadOnlyCollection<VariantListItemDto>>.Fail(ProductErrors.NotFound);
        }

        var items = _mapper.Map<IReadOnlyCollection<VariantListItemDto>>(product.Variants.OrderBy(x => x.Sku));

        return Result<IReadOnlyCollection<VariantListItemDto>>.Success(items);
    }
}
