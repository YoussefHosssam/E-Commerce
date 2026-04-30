using E_Commerce.Application.Contracts.Persistence;
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
    internal class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private DbSet<Cart> _cartDbSet;
        public CartRepository(EcommerceContext ctx) : base(ctx)
        {
            _cartDbSet = ctx.Carts;
        }
        public async Task<Cart?> GetCartWithItemsByToken(string token, CancellationToken ctn)
        {
            return await _cartDbSet.Include(c => c.Items).FirstOrDefaultAsync(x => x.AnonymousToken == token && x.Status == CartStatus.Active, ctn);
        }
        public async Task<Cart?> GetCartWithItemsByUserId(Guid id, CancellationToken ctn)
        {
            return await _cartDbSet.Include(c => c.Items).FirstOrDefaultAsync(x => x.UserId == id && x.Status == CartStatus.Active, ctn);
        }
    }
}
