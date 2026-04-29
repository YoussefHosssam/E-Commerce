using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastrucuture.Auth.RefreshTokens
{
    public interface IRefreshTokenService
    {
        Task<(string , TokenHash , DateTimeOffset)> IssueAsync(Guid userId, CancellationToken ct);
    }
}
