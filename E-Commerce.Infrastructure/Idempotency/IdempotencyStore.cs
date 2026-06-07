using E_Commerce.Application.Contracts.Infrastructure.Cache;
using E_Commerce.Application.Contracts.Infrastructure.Idempotency;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Idempotency
{
    public class IdempotencyStore : IIdempotencyStore
    {
        private readonly IDistributedCacheService _cache;
        private readonly TimeSpan _timeSpan;

        public IdempotencyStore(IDistributedCacheService cache)
        {
            _cache = cache;
            _timeSpan = TimeSpan.FromMinutes(10);
        }

        public async Task<IdempotencyRecord?> GetAsync(Guid userId, string operation, string idempotencyKey, CancellationToken ct)
        {
            string key = GenerateIdempotencyCacheKey(userId, idempotencyKey, operation);
            return await _cache.GetAsync<IdempotencyRecord>(key, ct);
        }

        public async Task MarkCompletedAsync(
            Guid userId,
            string operation,
            string idempotencyKey,
            int statusCode,
            string? responseBodyJson,
            string contentType,
            CancellationToken ct)
        {
            var record = await GetAsync(userId, operation, idempotencyKey, ct);

            if (record is null)
                return;

            record.MarkCompleted(statusCode, responseBodyJson, contentType);

            var key = GenerateIdempotencyCacheKey(userId, idempotencyKey, operation);

            await _cache.SetAsync(key, record, _timeSpan, ct);
        }

        public async Task MarkFailedAsync(Guid userId, string operation, string idempotencyKey, string failureReason, CancellationToken ct)
        {
            var idemRecord = await GetAsync(userId, operation, idempotencyKey, ct);
            idemRecord!.MarkFailed();
            string key = GenerateIdempotencyCacheKey(userId, idempotencyKey, operation);
            await _cache.SetAsync<IdempotencyRecord>(key, idemRecord, _timeSpan, ct);
        }

        public async Task<bool> TryBeginAsync(
            IdempotencyRecord record,
            CancellationToken ct)
        {
            var key = GenerateIdempotencyCacheKey(
                record.UserId,
                record.IdempotencyKey,
                record.Operation);

            return await _cache.SetIfNotExistsAsync(
                key,
                record,
                _timeSpan,
                ct);
        }

        private string GenerateIdempotencyCacheKey(Guid userId , string idempotencyKey , string operation)
        {
            return $"idempotency:{userId}:{operation}:{idempotencyKey}";
        }
    }
}
