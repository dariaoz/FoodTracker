using System.Text.Json;
using FoodTracker.Application.Repositories;
using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Notion.Client;
using FoodTracker.Infrastructure.Notion.Mappers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Repositories;

internal class ProductRepository : IProductRepository
{
    private readonly INotionClient _client;
    private readonly IDistributedCache _cache;
    private readonly string _databaseId;
    private const string CacheKeyPrefix = "product";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public ProductRepository(INotionClient client, IDistributedCache cache, IOptions<NotionOptions> options)
    {
        _client = client;
        _cache = cache;
        _databaseId = options.Value.ProductsDatabaseId;
    }

    public async Task<IList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        const string key = $"{CacheKeyPrefix}:all";
        string? cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<Product>>(cached)!;

        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        List<Product> products = db.Results.Select(ProductNotionMapper.ToEntity).ToList();
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(products), CacheOptions, ct);
        return products;
    }

    public async Task<Product?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}:{pageId}";
        string? cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<Product>(cached);

        NotionPage page = await _client.GetPageAsync(pageId, ct);
        Product product = ProductNotionMapper.ToEntity(page);
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(product), CacheOptions, ct);
        return product;
    }

    public async Task<Product> CreateAsync(Product entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.CreatePageAsync(_databaseId, ProductNotionMapper.ToNotionProperties(entity), ct);
        Product created = ProductNotionMapper.ToEntity(page);
        await InvalidateAsync(created.Id, ct);
        return created;
    }

    public async Task<Product> UpdateAsync(Product entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.UpdatePageAsync(entity.Id, ProductNotionMapper.ToNotionProperties(entity), ct);
        Product updated = ProductNotionMapper.ToEntity(page);
        await InvalidateAsync(updated.Id, ct);
        return updated;
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default)
    {
        await _client.ArchivePageAsync(pageId, ct);
        await InvalidateAsync(pageId, ct);
    }

    private async Task InvalidateAsync(string id, CancellationToken ct)
    {
        await _cache.RemoveAsync($"{CacheKeyPrefix}:{id}", ct);
        await _cache.RemoveAsync($"{CacheKeyPrefix}:all", ct);
    }
}
