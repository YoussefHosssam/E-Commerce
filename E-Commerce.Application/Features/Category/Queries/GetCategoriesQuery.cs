using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Category.Queries;

public sealed record GetCategoriesQuery(PageRequest page) : IRequest<Result<IReadOnlyCollection<CategoryListItemDto>>>;

public sealed class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyCollection<CategoryListItemDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<CategoryListItemDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _uow.Categories.GetAllOrderedAsync(request.page , cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CategoryListItemDto>>(categories.Items);
        return Result<IReadOnlyCollection<CategoryListItemDto>>.Success(items , categories.ToMetaResult());
    }
}
