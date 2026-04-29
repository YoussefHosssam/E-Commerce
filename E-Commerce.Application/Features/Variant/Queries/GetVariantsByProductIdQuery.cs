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
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, false, cancellationToken);
        if (product is null)
        {
            return Result<IReadOnlyCollection<VariantListItemDto>>.Fail(ErrorCatalog.FromCode(ErrorCodes.Product.NotFound));
        }

        var items = product.Variants
            .OrderBy(x => x.Sku)
            .Select(x => new VariantListItemDto(
                x.Id,
                x.ProductId,
                product.Slug.Value,
                x.Sku,
                x.Size,
                x.Color,
                x.PriceOverride is null ? null : E_Commerce.Application.Common.Dtos.MoneyDto.FromMoney(x.PriceOverride),
                x.IsActive))
            .ToArray();

        return Result<IReadOnlyCollection<VariantListItemDto>>.Success(items);
    }
}
