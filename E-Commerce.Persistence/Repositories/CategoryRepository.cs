using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence;
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

    
    public async Task<PagedResult<Category>> GetAllOrderedAsync(PageRequest page , CancellationToken ct)
    {
        var AllPagedCategories = await  _categories
            .AsNoTracking()
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .Include(x => x.Products)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Slug.Value)
            .ToPagedResultAsync(page , ct);
        return AllPagedCategories;
    }

    public async Task<Category?> GetByIdWithDetailsAsync(Guid id, bool asTracking, CancellationToken ct)
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
}
