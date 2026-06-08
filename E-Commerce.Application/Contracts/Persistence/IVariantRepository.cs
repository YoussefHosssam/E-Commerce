using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IVariantRepository : IGenericRepository<Variant>
{
    Task<Variant?> GetAggregateByIdAsync(Guid variantId, CancellationToken ct);
    Task<Variant?> GetAggregateByIdAsync(Guid variantId, bool asTracking, CancellationToken ct);
    Task<VariantDetailDto?> GetVariantDetailsDtoAsync(Guid productId, Guid variantId, CancellationToken ct);
    Task<IReadOnlyCollection<VariantListItemDto>?> GetVariantListItemDtosByProductIdAsync(Guid productId, CancellationToken ct);
    Task<IReadOnlyCollection<Variant>> GetByProductIdAsync(Guid productId, CancellationToken ct);
    Task<bool> SkuExistsAsync(string sku, Guid? excludedVariantId, CancellationToken ct);
    Task<bool> VariantExistsAsync(Guid id, CancellationToken ct);
    Task<bool> IsVariantReferencedAsync(Guid variantId, CancellationToken ct);
    Task<bool> IsVariantUsedInOrdersAsync(Guid variantId, CancellationToken ct);
    Task<bool> IsVariantUsedInActiveCartsAsync(Guid variantId, CancellationToken ct);
}
