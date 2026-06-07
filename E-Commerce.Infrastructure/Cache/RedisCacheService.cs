using System.Text.Json;
using E_Commerce.Application.Contracts.Infrastructure.Cache;
using StackExchange.Redis;

namespace E_Commerce.Infrastructure.Cache;

internal sealed class RedisCacheService : IDistributedCacheService
{
    private readonly IDatabase _db;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!, JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan ttl,
        CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);

        await _db.StringSetAsync(
            key,
            json,
            expiry: ttl);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> SetIfNotExistsAsync<T>(
    string key,
    T value,
    TimeSpan ttl,
    CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);

        return await _db.StringSetAsync(
            key,
            json,
            expiry: ttl,
            when: When.NotExists);
    }
}