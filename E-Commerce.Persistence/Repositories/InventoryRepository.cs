using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
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
        private readonly DbSet<Inventory> _inventoryDbSet;

        public InventoryRepository(EcommerceContext ecommerceContext) : base(ecommerceContext)
        {
            _inventoryDbSet = ecommerceContext.Set<Inventory>();
        }

        public Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ctn)
        {
            return _inventoryDbSet.FirstOrDefaultAsync(i => i.VariantId == variantId);
        }

        public async Task<bool> IsQuantityValid (Guid variantId , int quantity , CancellationToken ctn)
        {
            var inventory = await _inventoryDbSet.AsNoTracking().FirstOrDefaultAsync(i => i.VariantId == variantId , ctn);
            var existQuantity = inventory!.Available - inventory.Reserved;
            return quantity >= existQuantity;
        }
        public async Task<int> GetQuantityForVariant(Guid variantId, CancellationToken ctn)
        {
            var inventory = await _inventoryDbSet.AsNoTracking().Select(i => new {i.VariantId , i.Available , i.Reserved}).FirstOrDefaultAsync(i => i.VariantId == variantId, ctn);
            var existQuantity = inventory!.Available - inventory.Reserved;
            return existQuantity;
        }
    }
}
