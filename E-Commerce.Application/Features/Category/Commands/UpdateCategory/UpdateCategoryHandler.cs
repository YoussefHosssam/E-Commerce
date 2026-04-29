using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Category.Common;
using DomainCategory = E_Commerce.Domain.Entities.Category;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDetailDto>>
{
    private readonly IUnitOfWork _uow;

    public UpdateCategoryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<CategoryDetailDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdWithDetailsAsync(request.Id, true, cancellationToken);
        if (category is null)
        {
            return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.NotFound));
        }

        if (request.ParentId == request.Id)
        {
            return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.ParentSelf));
        }

        DomainCategory? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _uow.Categories.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
            {
                return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.ParentNotFound));
            }

            if (await _uow.Categories.IsDescendantAsync(category.Id, request.ParentId.Value, cancellationToken))
            {
                return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.ParentCycle));
            }
        }

        var slug = Slug.Create(request.Slug);
        if (await _uow.Categories.SlugExistsAsync(slug, category.Id, cancellationToken))
        {
            return Result<CategoryDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Category.SlugDuplicate));
        }

        category.ChangeSlug(slug);
        category.SetSortOrder(request.SortOrder);
        category.ChangeParent(parent);

        if (request.IsActive)
        {
            category.Activate();
        }
        else
        {
            category.Deactivate();
        }

        await _uow.SaveChangesAsync(cancellationToken);
        var updatedCategory = await _uow.Categories.GetByIdWithDetailsAsync(category.Id, false, cancellationToken) ?? category;
        return Result<CategoryDetailDto>.Success(updatedCategory.ToDetailDto());
    }
}

