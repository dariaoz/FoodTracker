using System.Text.Json;
using FoodTracker.Application.Repositories;
using FoodTracker.Domain.Interfaces;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Repositories;

internal class CachedRepository<T>(
    IRepository<T> inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options) : IRepository<T> where T : IHaveId
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
            return JsonSerializer.Deserialize<List<T>>(cached)!;

        IList<T> items = await inner.GetAllAsync(ct);
        await cache.SetStringAsync(key, JsonSerializer.Serialize(items), _cacheOptions, ct);
        return items;
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        string key = $"{_prefix}:{id}";
        string? cached = await cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<T>(cached);

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
        await cache.RemoveAsync($"{_prefix}:{id}", ct);
        await cache.RemoveAsync($"{_prefix}:all", ct);
    }
}
