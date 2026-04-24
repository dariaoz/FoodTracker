using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Domain.Recipes;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Notion;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Recipes;

internal class RecipeRepository : IRecipeRepository
{
    private readonly INotionClient _client;
    private readonly string _databaseId;

    public RecipeRepository(INotionClient client, IOptions<NotionOptions> options)
    {
        _client = client;
        _databaseId = options.Value.RecipesDatabaseId;
    }

    public async Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default)
    {
        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        return db.Results.Select(RecipeNotionMapper.ToEntity).ToList();
    }

    public async Task<Recipe?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        NotionPage page = await _client.GetPageAsync(pageId, ct);
        return RecipeNotionMapper.ToEntity(page);
    }

    public async Task<Recipe> CreateAsync(Recipe entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.CreatePageAsync(_databaseId, RecipeNotionMapper.ToNotionProperties(entity), ct);
        return RecipeNotionMapper.ToEntity(page);
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default) =>
        await _client.DeletePageAsync(_databaseId, pageId, ct);
}
