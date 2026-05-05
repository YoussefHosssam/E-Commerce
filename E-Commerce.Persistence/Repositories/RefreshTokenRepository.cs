using E_Commerce.Application.Contracts.Persistence;
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
    internal class RefreshTokenRepository : GenericRepository<RefreshToken> , IRefreshTokenRepository
    {
        private readonly DbSet<RefreshToken> _refreshTokens;
        public RefreshTokenRepository(EcommerceContext ecommerceContext) : base(ecommerceContext)
        {
            _refreshTokens = ecommerceContext.Set<RefreshToken>();
        }
        public async Task<RefreshToken?> GetByHashedTokenAsync(TokenHash hashedToken, CancellationToken cancellationToken)
        {
            return await _refreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hashedToken);
        }
    }
}
