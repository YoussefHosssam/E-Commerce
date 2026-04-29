using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IVariantRepository : IGenericRepository<Variant>
{
    Task<Variant?> GetByIdWithDetailsAsync(Guid variantId, CancellationToken ct);
    Task<IReadOnlyCollection<Variant>> GetByProductIdAsync(Guid productId, CancellationToken ct);
    Task<bool> SkuExistsAsync(string sku, Guid? excludedVariantId, CancellationToken ct);
    Task<bool> IsVariantReferencedAsync(Guid variantId, CancellationToken ct);
}
