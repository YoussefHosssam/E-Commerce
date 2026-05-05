using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence.Shared
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ctn);
        Task<bool> IsQuantityValid(Guid variantId, int quantity, CancellationToken ctn);
        Task<int> GetQuantityForVariant(Guid variantId, CancellationToken ctn);
        Task<IEnumerable<Inventory?>> GetByVariantIdsAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken ct);
    }
}
