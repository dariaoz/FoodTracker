using System.Text.Json;
using FoodTracker.Application.Repositories;
using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Notion.Client;
using FoodTracker.Infrastructure.Notion.Mappers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Repositories;

internal class FoodLogRepository : IFoodLogRepository
{
    private readonly INotionClient _client;
    private readonly IDistributedCache _cache;
    private readonly string _databaseId;
    private const string CacheKeyPrefix = "foodlog";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public FoodLogRepository(INotionClient client, IDistributedCache cache, IOptions<NotionOptions> options)
    {
        _client = client;
        _cache = cache;
        _databaseId = options.Value.FoodLogDatabaseId;
    }

    public async Task<IList<FoodLog>> GetAllAsync(CancellationToken ct = default)
    {
        const string key = $"{CacheKeyPrefix}:all";
        string? cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<FoodLog>>(cached)!;

        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        List<FoodLog> logs = await MapPagesAsync(db.Results, ct);
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(logs), CacheOptions, ct);
        return logs;
    }

    public async Task<FoodLog?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}:{pageId}";
        string? cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<FoodLog>(cached);

        NotionPage page = await _client.GetPageAsync(pageId, ct);
        FoodLog log = await MapPageAsync(page, ct);
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(log), CacheOptions, ct);
        return log;
    }

    public async Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        var filter = new
        {
            property = "Date",
            date = new { equals = date.ToString("yyyy-MM-dd") }
        };
        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, filter, ct);
        return await MapPagesAsync(db.Results, ct);
    }

    public async Task<FoodLog> CreateAsync(FoodLog entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.CreatePageAsync(_databaseId, FoodLogNotionMapper.ToNotionProperties(entity), ct);
        FoodLog created = await MapPageAsync(page, ct);
        await InvalidateAsync(created.Id, ct);
        return created;
    }

    public async Task<FoodLog> UpdateAsync(FoodLog entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.UpdatePageAsync(entity.Id, FoodLogNotionMapper.ToNotionProperties(entity), ct);
        FoodLog updated = await MapPageAsync(page, ct);
        await InvalidateAsync(updated.Id, ct);
        return updated;
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default)
    {
        await _client.ArchivePageAsync(pageId, ct);
        await InvalidateAsync(pageId, ct);
    }

    private async Task<FoodLog> MapPageAsync(NotionPage page, CancellationToken ct)
    {
        Dictionary<string, NotionPropertyValue> props = page.Properties;
        string recipeId = FoodLogNotionMapper.GetLinkedRecipeId(props);
        string productId = FoodLogNotionMapper.GetLinkedProductId(props);

        Recipe? recipe = null;
        Product? product = null;

        if (!string.IsNullOrEmpty(recipeId))
        {
            NotionPage recipePage = await _client.GetPageAsync(recipeId, ct);
            recipe = RecipeNotionMapper.ToEntity(recipePage);
        }
        else if (!string.IsNullOrEmpty(productId))
        {
            NotionPage productPage = await _client.GetPageAsync(productId, ct);
            product = ProductNotionMapper.ToEntity(productPage);
        }

        return FoodLogNotionMapper.ToEntity(page, product, recipe);
    }

    private async Task<List<FoodLog>> MapPagesAsync(IEnumerable<NotionPage> pages, CancellationToken ct) =>
        (await Task.WhenAll(pages.Select(p => MapPageAsync(p, ct)))).ToList();

    private async Task InvalidateAsync(string id, CancellationToken ct)
    {
        await _cache.RemoveAsync($"{CacheKeyPrefix}:{id}", ct);
        await _cache.RemoveAsync($"{CacheKeyPrefix}:all", ct);
    }
}
