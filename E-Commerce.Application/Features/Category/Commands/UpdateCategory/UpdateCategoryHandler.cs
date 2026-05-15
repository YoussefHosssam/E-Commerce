using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Category.Common;
using DomainCategory = E_Commerce.Domain.Entities.Category;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using E_Commerce.Domain.Common.Errors;
using AutoMapper;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateCategoryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDetailDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var category = await _uow.Categories.GetByIdWithDetailsAsync(request.Id, true, cancellationToken);
        if (category is null)
        {
            return Result<CategoryDetailDto>.Fail(CategoryErrors.NotFound);
        }

        if (request.ParentId == request.Id)
        {
            return Result<CategoryDetailDto>.Fail(CategoryErrors.ParentSelf);
        }

        DomainCategory? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _uow.Categories.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
            {
                return Result<CategoryDetailDto>.Fail(CategoryErrors.ParentNotFound);
            }

            if (await _uow.Categories.IsDescendantAsync(category.Id, request.ParentId.Value, cancellationToken))
            {
                return Result<CategoryDetailDto>.Fail(CategoryErrors.ParentCycle);
            }
        }

        var slug = Slug.Create(request.Slug);
        if (await _uow.Categories.SlugExistsAsync(slug, category.Id, cancellationToken))
        {
            return Result<CategoryDetailDto>.Fail(CategoryErrors.SlugDuplicate);
        }

        var name = request.Name;
        if (await _uow.Categories.NameExistsAsync(name, category.Id, cancellationToken))
        {
            return Result<CategoryDetailDto>.Fail(CategoryErrors.NameAlreadyExsit);
        }
        category.ChangeName(name, now);
        category.ChangeSlug(slug , now);
        category.SetSortOrder(request.SortOrder , now);
        category.ChangeParent(parent , now);

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
        return Result<CategoryDetailDto>.Success(_mapper.Map<CategoryDetailDto>(updatedCategory));
    }
}

