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

        public async Task<Inventory?> GetByVariantIdAsync(Guid variantId, CancellationToken ctn)
        {
            return await _inventoryDbSet.FirstOrDefaultAsync(i => i.VariantId == variantId);
        }
    }
}
