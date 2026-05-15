using E_Commerce.Application.Contracts.Persistence;
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

    public async Task<Variant?> GetByIdWithDetailsAsync(Guid variantId, CancellationToken ct)
    {
        return await GetByIdWithDetailsAsync(variantId, false, ct);
    }

    public async Task<Variant?> GetByIdWithDetailsAsync(Guid variantId, bool asTracking, CancellationToken ct)
    {
        IQueryable<Variant> query = _variants;

        if (!asTracking)
            query = query.AsNoTracking();

        return await query
            .Include(x => x.Product)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == variantId, ct);
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
}
