using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByHashedTokenAsync(TokenHash hashedToken, CancellationToken cancellationToken);
    }
}
