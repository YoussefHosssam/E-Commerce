using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Cache
{
    public interface IDistributedCacheService : ICacheService
    {
        Task<bool> SetIfNotExistsAsync<T>(
            string key,
            T value,
            TimeSpan ttl,
            CancellationToken ct = default);
    }
}
