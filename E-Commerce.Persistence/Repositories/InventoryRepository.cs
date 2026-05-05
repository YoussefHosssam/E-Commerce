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
    internal class InventoryRepository : GenericRepository<Inventory> , IInventoryRepository
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

        public async Task<bool> IsQuantityValid (Guid variantId , int quantity , CancellationToken ctn)
        {
            var inventory = await _inventories.AsNoTracking().FirstOrDefaultAsync(i => i.VariantId == variantId , ctn);
            var existQuantity = inventory!.Available;
            return existQuantity >= quantity;
        }
        public async Task<int> GetQuantityForVariant(Guid variantId, CancellationToken ctn)
        {
            var inventory = await _inventories.AsNoTracking().Select(i => new {i.VariantId , i.OnHand , i.Reserved , i.Available}).FirstOrDefaultAsync(i => i.VariantId == variantId, ctn);
            return inventory!.Available;
        }

        public async Task<IEnumerable<Inventory?>> GetByVariantIdsAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken ct)
        {
            return await _inventories.Where(i => variantIds.Contains(i.VariantId)).ToListAsync(ct);
        }
    }
}
