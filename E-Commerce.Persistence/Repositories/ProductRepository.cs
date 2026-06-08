using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Product.Common;
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

    public async Task<PagedResult<ProductListItemDto>> GetProductListItemDtosAsync(PageRequest pageRequest, CancellationToken ct)
    {
        return await _products
            .AsNoTracking()
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
            .ToPagedResultAsync(pageRequest, ct);
    }

    public async Task<ProductDetailDto?> GetProductDetailsDtoAsync(Guid id, CancellationToken ct)
    {
        return await _products
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductDetailDto
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
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Variants = x.Variants
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.Sku)
                    .Select(v => new ProductVariantDto
                    {
                        Id = v.Id,
                        Sku = v.Sku,
                        Size = v.Size,
                        Color = new ColorDto(v.Color.Name, v.Color.HexCode),
                        EffectivePrice = new MoneyDto(
                            v.Price == null ? x.BasePrice.Amount : v.Price.Amount,
                            v.Price == null ? x.BasePrice.Currency.Value : v.Price.Currency.Value),
                        VariantPriceOverride = v.Price == null
                            ? null
                            : new MoneyDto(v.Price.Amount, v.Price.Currency.Value),
                        Stock = v.Inventory == null ? 0 : v.Inventory.Available,
                        IsDefault = v.IsDefault,
                        IsActive = v.IsActive
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Product?> GetAggregateByIdAsync(Guid id, bool asTracking, CancellationToken ct)
    {
        IQueryable<Product> query = _products;

        if (!asTracking)
            query = query.AsNoTracking();

        return await query
            .Include(x => x.Category)
            .Include(x => x.Variants)
                .ThenInclude(x => x.Inventory)
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
