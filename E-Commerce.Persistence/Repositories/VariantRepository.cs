using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

internal sealed class VariantRepository : GenericRepository<Variant>, IVariantRepository
{
    private readonly EcommerceContext _context;
    private readonly DbSet<Variant> _variants;

    public VariantRepository(EcommerceContext context) : base(context)
    {
        _context = context;
        _variants = _context.Set<Variant>();
    }

    public async Task<Variant?> GetAggregateByIdAsync(Guid variantId, CancellationToken ct)
    {
        return await GetAggregateByIdAsync(variantId, false, ct);
    }

    public async Task<Variant?> GetAggregateByIdAsync(Guid variantId, bool asTracking, CancellationToken ct)
    {
        IQueryable<Variant> query = _variants;

        if (!asTracking)
            query = query.AsNoTracking();

        return await query
            .Include(x => x.Product)
            .Include(x => x.Inventory)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == variantId, ct);
    }

    public async Task<VariantDetailDto?> GetVariantDetailsDtoAsync(Guid productId, Guid variantId, CancellationToken ct)
    {
        return await _variants
            .AsNoTracking()
            .Where(x => x.Id == variantId && x.ProductId == productId)
            .Select(x => new VariantDetailDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductSlug = x.Product.Slug.Value,
                Sku = x.Sku,
                Size = x.Size,
                Color = new ColorDto(x.Color.Name, x.Color.HexCode),
                EffectivePrice = new MoneyDto(
                    x.Price == null ? x.Product.BasePrice.Amount : x.Price.Amount,
                    x.Price == null ? x.Product.BasePrice.Currency.Value : x.Price.Currency.Value),
                VariantPriceOverride = x.Price == null
                    ? null
                    : new MoneyDto(x.Price.Amount, x.Price.Currency.Value),
                Stock = x.Inventory == null ? 0 : x.Inventory.Available,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<VariantListItemDto>?> GetVariantListItemDtosByProductIdAsync(Guid productId, CancellationToken ct)
    {
        var productExists = await _context.Products
            .AsNoTracking()
            .AnyAsync(x => x.Id == productId, ct);

        if (!productExists)
            return null;

        return await _variants
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.Sku)
            .Select(x => new VariantListItemDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductSlug = x.Product.Slug.Value,
                Sku = x.Sku,
                Size = x.Size,
                Color = new ColorDto(x.Color.Name, x.Color.HexCode),
                EffectivePrice = new MoneyDto(
                    x.Price == null ? x.Product.BasePrice.Amount : x.Price.Amount,
                    x.Price == null ? x.Product.BasePrice.Currency.Value : x.Price.Currency.Value),
                VariantPriceOverride = x.Price == null
                    ? null
                    : new MoneyDto(x.Price.Amount, x.Price.Currency.Value),
                Stock = x.Inventory == null ? 0 : x.Inventory.Available,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<Variant>> GetByProductIdAsync(Guid productId, CancellationToken ct)
    {
        return await _variants
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.Sku)
            .ToListAsync(ct);
    }

    public Task<bool> SkuExistsAsync(string sku, Guid? excludedVariantId, CancellationToken ct)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();

        return _variants
            .AsNoTracking()
            .AnyAsync(
                x => x.Sku == normalizedSku && (!excludedVariantId.HasValue || x.Id != excludedVariantId.Value),
                ct);
    }

    public Task<bool> VariantExistsAsync(Guid id, CancellationToken ct)
    {
        return _variants
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == id,
                ct);
    }

    public async Task<bool> IsVariantReferencedAsync(Guid variantId, CancellationToken ct)
    {
        return await _context.OrderItems.AsNoTracking().AnyAsync(x => x.VariantId == variantId, ct)
            || await _context.CartItems.AsNoTracking().AnyAsync(x => x.VariantId == variantId, ct)
            || await _context.StockAlerts.AsNoTracking().AnyAsync(x => x.VariantId == variantId, ct)
            || await _context.StockMovements.AsNoTracking().AnyAsync(x => x.VariantId == variantId, ct);
    }

    public Task<bool> IsVariantUsedInOrdersAsync(Guid variantId, CancellationToken ct)
    {
        return _context.OrderItems.AsNoTracking().AnyAsync(x => x.VariantId == variantId, ct);
    }

    public Task<bool> IsVariantUsedInActiveCartsAsync(Guid variantId, CancellationToken ct)
    {
        return _context.CartItems
            .AsNoTracking()
            .AnyAsync(x => x.VariantId == variantId && x.Cart.Status != CartStatus.CheckedOut, ct);
    }
}
