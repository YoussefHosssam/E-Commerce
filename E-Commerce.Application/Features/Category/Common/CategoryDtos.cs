using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Category.Common;

public sealed record CategoryListItemDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Slug,
    int SortOrder,
    bool IsActive,
    int ChildrenCount,
    int ProductsCount);

public sealed record CategoryChildDto(Guid Id, string Name, string Slug, int SortOrder, bool IsActive);

public sealed record CategoryDetailDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string? ParentSlug,
    string Slug,
    int SortOrder,
    bool IsActive,
    int ProductsCount,
    IReadOnlyCollection<CategoryChildDto> Children);
