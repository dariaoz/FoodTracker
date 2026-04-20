using System.Text.Json;
using FoodTracker.Api.Configuration;
using FoodTracker.Api.Domain.Entities;
using FoodTracker.Api.Notion.Client;
using FoodTracker.Api.Notion.Mappers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Api.Notion.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly INotionClient _client;
    private readonly IDistributedCache _cache;
    private readonly string _databaseId;
    private const string CacheKeyPrefix = "recipe";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public RecipeRepository(INotionClient client, IDistributedCache cache, IOptions<NotionOptions> options)
    {
        _client = client;
        _cache = cache;
        _databaseId = options.Value.RecipesDatabaseId;
    }

    public async Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default)
    {
        const string key = $"{CacheKeyPrefix}:all";
        string? cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<Recipe>>(cached)!;

        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        List<Recipe> recipes = db.Results.Select(RecipeNotionMapper.ToEntity).ToList();
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(recipes), CacheOptions, ct);
        return recipes;
    }

    public async Task<Recipe?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        string key = $"{CacheKeyPrefix}:{pageId}";
        string? cached = await _cache.GetStringAsync(key, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<Recipe>(cached);

        NotionPage page = await _client.GetPageAsync(pageId, ct);
        Recipe recipe = RecipeNotionMapper.ToEntity(page);
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(recipe), CacheOptions, ct);
        return recipe;
    }

    public async Task<Recipe> CreateAsync(Recipe entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.CreatePageAsync(_databaseId, RecipeNotionMapper.ToNotionProperties(entity), ct);
        Recipe created = RecipeNotionMapper.ToEntity(page);
        await InvalidateAsync(created.Id, ct);
        return created;
    }

    public async Task<Recipe> UpdateAsync(Recipe entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.UpdatePageAsync(entity.Id, RecipeNotionMapper.ToNotionProperties(entity), ct);
        Recipe updated = RecipeNotionMapper.ToEntity(page);
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
