using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

internal sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly EcommerceContext _context;
    private readonly DbSet<Category> _categories;

    public CategoryRepository(EcommerceContext context) : base(context)
    {
        _context = context;
        _categories = _context.Set<Category>();
    }

    
    public async Task<PagedResult<CategoryListItemDto>> GetCategoryListItemDtosAsync(PageRequest page , CancellationToken ct)
    {
        var AllPagedCategories = await  _categories
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Slug)
            .Select(x => new CategoryListItemDto(
                x.Id,
                x.ParentId,
                x.Name,
                x.Slug.Value,
                x.SortOrder,
                x.IsActive,
                x.Children.Count,
                x.Products.Count))
            .ToPagedResultAsync(page , ct);
        return AllPagedCategories;
    }

    public async Task<CategoryDetailDto?> GetCategoryDetailsDtoAsync(Guid id, CancellationToken ct)
    {
        return await _categories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CategoryDetailDto(
                x.Id,
                x.ParentId,
                x.Name,
                x.Parent == null ? null : x.Parent.Slug.Value,
                x.Slug.Value,
                x.SortOrder,
                x.IsActive,
                x.Products.Count,
                x.Children
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.Slug.Value)
                    .Select(c => new CategoryChildDto(
                        c.Id,
                        c.Name,
                        c.Slug.Value,
                        c.SortOrder,
                        c.IsActive))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Category?> GetAggregateByIdAsync(Guid id, bool asTracking, CancellationToken ct)
    {
        IQueryable<Category> query = _categories;

        if (!asTracking)
            query = query.AsNoTracking();

        return await query
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> SlugExistsAsync(Slug slug, Guid? excludedId, CancellationToken ct)
    {
        return _categories
            .AsNoTracking()
            .AnyAsync(
                x => x.Slug == slug && (!excludedId.HasValue || x.Id != excludedId.Value),
                ct);
    }

    public Task<bool> HasProductsAsync(Guid categoryId, CancellationToken ct)
        => _context.Products.AsNoTracking().AnyAsync(x => x.CategoryId == categoryId, ct);

    public Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken ct)
        => _categories.AsNoTracking().AnyAsync(x => x.ParentId == categoryId, ct);

    public async Task<bool> IsDescendantAsync(Guid categoryId, Guid potentialParentId, CancellationToken ct)
    {
        var currentParentId = await _categories
            .AsNoTracking()
            .Where(x => x.Id == potentialParentId)
            .Select(x => x.ParentId)
            .FirstOrDefaultAsync(ct);

        while (currentParentId.HasValue)
        {
            if (currentParentId.Value == categoryId)
                return true;

            currentParentId = await _categories
                .AsNoTracking()
                .Where(x => x.Id == currentParentId.Value)
                .Select(x => x.ParentId)
                .FirstOrDefaultAsync(ct);
        }

        return false;
    }

    public Task<bool> NameExistsAsync(string name, Guid? excludedId, CancellationToken ct)
    {
        return _categories
            .AsNoTracking()
            .AnyAsync(
        x => x.Name == name && (!excludedId.HasValue || x.Id != excludedId.Value),
        ct);
    }

    public async Task<PagedResult<ProductListItemDto>?> GetCategoryProductListItemDtosAsync(Guid id, PageRequest page, CancellationToken ct)
    {
        var categoryExists = await _categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == id && c.IsActive, ct);

        if (!categoryExists)
            return null;

        return await _context.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ProductListItemDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Name = x.Name,
                CategorySlug = x.Category.Slug.Value,
                Slug = x.Slug.Value,
                Brand = x.Brand,
                Status = x.Status.ToString(),
                BasePrice = new MoneyDto(x.BasePrice.Amount, x.BasePrice.Currency.Value),
                HasVariants = x.HasVariants,
                HasDiscount = x.HasDiscount,
                CompareAtPrice = x.CompareAtPrice == null
                    ? null
                    : new MoneyDto(x.CompareAtPrice.Amount, x.CompareAtPrice.Currency.Value),
                DefaultVariantId = x.Variants
                    .Where(v => v.IsActive && v.IsDefault)
                    .Select(v => (Guid?)v.Id)
                    .FirstOrDefault(),
                MinPrice = x.Variants
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.Price == null ? x.BasePrice.Amount : v.Price.Amount)
                    .Select(v => new MoneyDto(
                        v.Price == null ? x.BasePrice.Amount : v.Price.Amount,
                        v.Price == null ? x.BasePrice.Currency.Value : v.Price.Currency.Value))
                    .FirstOrDefault(),
                MaxPrice = x.Variants
                    .Where(v => v.IsActive)
                    .OrderByDescending(v => v.Price == null ? x.BasePrice.Amount : v.Price.Amount)
                    .Select(v => new MoneyDto(
                        v.Price == null ? x.BasePrice.Amount : v.Price.Amount,
                        v.Price == null ? x.BasePrice.Currency.Value : v.Price.Currency.Value))
                    .FirstOrDefault(),
                IsActive = x.IsActive,
                VariantCount = x.Variants.Count
            })
            .ToPagedResultAsync(page, ct);
    }
}
