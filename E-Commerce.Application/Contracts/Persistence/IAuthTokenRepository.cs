using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence
{
    public interface IAuthTokenRepository : IGenericRepository<AuthToken>
    {
        public Task<AuthToken?> GetLastRelatedToken(Guid userId, TokenType tokenType, CancellationToken ctn);
    }
}
