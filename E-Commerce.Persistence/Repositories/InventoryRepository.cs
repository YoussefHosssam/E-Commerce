using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    internal class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        private readonly DbSet<Inventory> _inventories;

        public InventoryRepository(EcommerceContext ecommerceContext) : base(ecommerceContext)
        {
            _inventories = ecommerceContext.Set<Inventory>();
        }

        public Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ctn)
        {
            return _inventories.FirstOrDefaultAsync(i => i.VariantId == variantId);
        }

        public async Task<bool> IsQuantityValid(Guid variantId, int quantity, CancellationToken ctn)
        {
            return await _inventories.AsNoTracking().AnyAsync(i => i.VariantId == variantId && (i.OnHand - i.Reserved) >= quantity, ctn);
        }
        public async Task<int> GetQuantityForVariant(Guid variantId, CancellationToken ctn)
        {
            var inventory = await _inventories.AsNoTracking().Where(i => i.VariantId == variantId).Select(i => new { i.Reserved, i.OnHand, i.Available }).FirstOrDefaultAsync();
            return inventory!.Available;
        }

        public async Task<IEnumerable<Inventory?>> GetByVariantIdsAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken ct)
        {
            return await _inventories.Where(i => variantIds.Contains(i.VariantId)).ToListAsync(ct);
        }

        public async Task<bool> TryReserveAsync(
            Guid variantId,
            int quantity,
            DateTimeOffset now,
            CancellationToken ct)
        {
            var rows = await _inventories
                .Where(x =>
                    x.VariantId == variantId &&
                    x.OnHand - x.Reserved >= quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Reserved, x => x.Reserved + quantity)
                    .SetProperty(x => x.UpdatedAt, now),
                    ct);

            return rows == 1;
        }
    }
}
