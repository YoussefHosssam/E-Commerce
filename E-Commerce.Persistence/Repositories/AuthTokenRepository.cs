using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
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
    public class AuthTokenRepository : GenericRepository<AuthToken>, IAuthTokenRepository
    {
        private readonly DbSet<AuthToken> _authTokens;
        public AuthTokenRepository(EcommerceContext ecommerceContext) : base(ecommerceContext)
        {
            _authTokens = ecommerceContext.Set<AuthToken>();
        }
        public async Task<AuthToken?> GetLastRelatedToken(Guid userId, TokenType tokenType, CancellationToken ctn)
        {
            AuthToken? lastRelatedAuthToken = await _authTokens.OrderBy(at => at.CreatedAt).LastOrDefaultAsync( at => at.UserId == userId && at.TokenType == tokenType , ctn);
            return lastRelatedAuthToken;
        }
    }
}
