using E_Commerce.Application.Contracts.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Cache
{
    internal class MemoryCacheService : ILocalCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            _memoryCache.TryGetValue(key, out T? value);

            return Task.FromResult(value);
        }

        public Task SetAsync<T>(
            string key,
            T value,
            TimeSpan ttl,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            _memoryCache.Set(key, value, options);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            _memoryCache.Remove(key);

            return Task.CompletedTask;
        }
    }
}
