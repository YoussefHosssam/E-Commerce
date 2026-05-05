using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Product.Common;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Product.Queries.GetProducts;

public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<List<ProductListItemDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductsHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<List<ProductListItemDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _uow.Products.GetPagedProductsWithDetailsAsync(request.page, cancellationToken);

        List<ProductListItemDto> items = _mapper.Map<List<ProductListItemDto>>(products.Items);

        return Result<List<ProductListItemDto>>.Success(items , products.ToMetaResult());
    }
}
