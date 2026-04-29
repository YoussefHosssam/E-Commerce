using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Category.Common;

public sealed record CategoryListItemDto(
    Guid Id,
    Guid? ParentId,
    string Slug,
    int SortOrder,
    bool IsActive,
    int ChildrenCount,
    int ProductsCount);

public sealed record CategoryChildDto(Guid Id, string Slug, int SortOrder, bool IsActive);

public sealed record CategoryDetailDto(
    Guid Id,
    Guid? ParentId,
    string? ParentSlug,
    string Slug,
    int SortOrder,
    bool IsActive,
    int ProductsCount,
    IReadOnlyCollection<CategoryChildDto> Children);

internal static class CategoryDtoMappings
{
    public static CategoryListItemDto ToListItemDto(this E_Commerce.Domain.Entities.Category category)
        => new(
            category.Id,
            category.ParentId,
            category.Slug.Value,
            category.SortOrder,
            category.IsActive,
            category.Children.Count,
            category.Products.Count);

    public static CategoryDetailDto ToDetailDto(this E_Commerce.Domain.Entities.Category category)
        => new(
            category.Id,
            category.ParentId,
            category.Parent?.Slug.Value,
            category.Slug.Value,
            category.SortOrder,
            category.IsActive,
            category.Products.Count,
            category.Children
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Slug.Value)
                .Select(x => new CategoryChildDto(x.Id, x.Slug.Value, x.SortOrder, x.IsActive))
                .ToArray());
}


