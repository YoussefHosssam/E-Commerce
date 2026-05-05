using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

internal class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly DbSet<Product> _products;

    public ProductRepository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {
        _products = ecommerceContext.Set<Product>();
    }

    public async Task<PagedResult<Product>> GetPagedProducts(PageRequest pageRequest, CancellationToken ctn)
    {
        return await _products.AsNoTracking().ToPagedResultAsync(pageRequest, ctn);
    }

    public async Task<PagedResult<Product>> GetPagedProductsWithDetailsAsync(PageRequest pageRequest, CancellationToken ct)
    {
        return await _products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .OrderByDescending(x => x.CreatedAt).ToPagedResultAsync(pageRequest, ct);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id, bool asTracking, CancellationToken ct)
    {
        IQueryable<Product> query = _products;

        if (!asTracking)
            query = query.AsNoTracking();

        return await query
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .ThenInclude(x => x.Images)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> SlugExistsAsync(Slug slug, Guid? excludedId, CancellationToken ct)
    {
        return _products
            .AsNoTracking()
            .AnyAsync(
                x => x.Slug == slug && (!excludedId.HasValue || x.Id != excludedId.Value),
                ct);
    }
}
