using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Category.Queries;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDetailDto>>;

public sealed class GetCategoryByIdValidation : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithError(ErrorCodes.Category.IdRequired);
    }
}

public sealed class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDetailDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCategoryByIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CategoryDetailDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdWithDetailsAsync(request.Id, false, cancellationToken);
        if (category is null)
        {
            return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.NotFound));
        }

        return Result<CategoryDetailDto>.Success(category.ToDetailDto());
    }
}
