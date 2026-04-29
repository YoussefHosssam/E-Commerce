using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Product.Common;
using MediatR;

namespace E_Commerce.Application.Features.Product.Queries.GetProducts;

public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<List<ProductListItemDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetProductsHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<ProductListItemDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _uow.Products.GetPagedProductsWithDetailsAsync(request.page, cancellationToken);

        List<ProductListItemDto> items = products.Items.Select(p => p.ToListItemDto()).ToList();

        return Result<List<ProductListItemDto>>.Success(items , products.ToMetaResult());
    }
}
