using System.Text.Json;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Shared;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Shared;

internal class CachedRepository<T>(
    IRepository<T> inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options,
    ILogger<CachedRepository<T>> logger) : IRepository<T> where T : IHaveId
{
    private readonly string _prefix = typeof(T).Name.ToLower();
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = options.Value.Ttl
    };

    public async Task<IList<T>> GetAllAsync(CancellationToken ct = default)
    {
        string key = $"{_prefix}:all";
        string? cached = await cache.GetStringAsync(key, ct);
        if (cached is not null)
        {
            logger.LogDebug("Cache hit for {Key}", key);
            return JsonSerializer.Deserialize<List<T>>(cached)!;
        }

        logger.LogDebug("Cache miss for {Key}", key);
        IList<T> items = await inner.GetAllAsync(ct);
        await cache.SetStringAsync(key, JsonSerializer.Serialize(items), _cacheOptions, ct);
        return items;
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        string key = $"{_prefix}:{id}";
        string? cached = await cache.GetStringAsync(key, ct);
        if (cached is not null)
        {
            logger.LogDebug("Cache hit for {Key}", key);
            return JsonSerializer.Deserialize<T>(cached);
        }

        logger.LogDebug("Cache miss for {Key}", key);
        T? item = await inner.GetByIdAsync(id, ct);
        if (item is not null)
            await cache.SetStringAsync(key, JsonSerializer.Serialize(item), _cacheOptions, ct);
        return item;
    }

    public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        T created = await inner.CreateAsync(entity, ct);
        await InvalidateAsync(created.Id, ct);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await inner.DeleteAsync(id, ct);
        await InvalidateAsync(id, ct);
    }

    private async Task InvalidateAsync(string id, CancellationToken ct)
    {
        logger.LogDebug("Invalidating cache for {Prefix}:{Id} and {Prefix}:all", _prefix, id, _prefix);
        await cache.RemoveAsync($"{_prefix}:{id}", ct);
        await cache.RemoveAsync($"{_prefix}:all", ct);
    }
}
