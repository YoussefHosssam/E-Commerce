using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Category.Queries;

public sealed record GetCategoriesQuery(PageRequest page) : IRequest<Result<IReadOnlyCollection<CategoryListItemDto>>>;

public sealed class GetCategoriesValidation : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesValidation()
    {
        RuleFor(x => x.page.PageNumber)
            .GreaterThan(0)
            .WithError(ErrorCodes.Common.PageInvalid);

        RuleFor(x => x.page.PageSize)
            .GreaterThan(0)
            .WithError(ErrorCodes.Common.PageSizeInvalid);
    }
}
public sealed class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyCollection<CategoryListItemDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetCategoriesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IReadOnlyCollection<CategoryListItemDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _uow.Categories.GetAllOrderedAsync(request.page , cancellationToken);
        var items = categories.Items.Select(x => x.ToListItemDto()).ToArray();
        return Result<IReadOnlyCollection<CategoryListItemDto>>.Success(items , categories.ToMetaResult());
    }
}
