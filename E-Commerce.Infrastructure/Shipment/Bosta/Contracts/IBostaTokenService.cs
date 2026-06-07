using E_Commerce.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Shipment.Bosta.Contracts
{
    public interface IBostaTokenService
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
        Task RefreshTokenAsync(CancellationToken cancellationToken = default);
    }
}
