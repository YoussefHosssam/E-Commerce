using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Category.Queries;

public record GetCategoryProductsQuery(Guid categoryId, PageRequest page) : IRequest<Result<IReadOnlyCollection<ProductListItemDto>>>;

public class GetCategoryProductsValidation : AbstractValidator<GetCategoryProductsQuery>
{
    public GetCategoryProductsValidation()
    {
        RuleFor(q => q.categoryId)
            .NotEmpty()
            .WithError(CategoryErrors.IdRequired);
    }
}

public class GetCategoryProductsHandler : IRequestHandler<GetCategoryProductsQuery, Result<IReadOnlyCollection<ProductListItemDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetCategoryProductsHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IReadOnlyCollection<ProductListItemDto>>> Handle(GetCategoryProductsQuery request, CancellationToken cancellationToken)
    {
        var categoryProducts = await _uow.Categories.GetCategoryProductListItemDtosAsync(request.categoryId, request.page, cancellationToken);
        if (categoryProducts is null)
            return Result<IReadOnlyCollection<ProductListItemDto>>.Fail(ProductErrors.NotFound);

        return Result<IReadOnlyCollection<ProductListItemDto>>.Success(categoryProducts.Items, categoryProducts.ToMetaResult());
    }
}
