using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Category.Common;
using DomainCategory = E_Commerce.Domain.Entities.Category;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDetailDto>>
{
    private readonly IUnitOfWork _uow;

    public CreateCategoryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CategoryDetailDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        DomainCategory? parent = null;

        if (request.ParentId.HasValue)
        {
            parent = await _uow.Categories.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
            {
                return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.ParentNotFound));
            }
        }

        var slug = Slug.Create(request.Slug);
        if (await _uow.Categories.SlugExistsAsync(slug, null, cancellationToken))
        {
            return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.SlugDuplicate));
        }

        var category = DomainCategory.Create(request.ParentId, slug, request.SortOrder);
        if (!request.IsActive)
        {
            category.Deactivate();
        }

        await _uow.Categories.CreateAsync(category, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var createdCategory = await _uow.Categories.GetByIdWithDetailsAsync(category.Id, false, cancellationToken) ?? category;
        return Result<CategoryDetailDto>.Success(createdCategory.ToDetailDto());
    }
}

