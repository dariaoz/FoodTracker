using FoodTracker.Application.FoodLogs;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Products;
using FoodTracker.Infrastructure.Recipes;
using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.FoodLogs;

internal class FoodLogRepository : IFoodLogRepository
{
    private readonly INotionClient _client;
    private readonly string _databaseId;

    public FoodLogRepository(INotionClient client, IOptions<NotionOptions> options)
    {
        _client = client;
        _databaseId = options.Value.FoodLogDatabaseId;
    }

    public async Task<IList<FoodLog>> GetAllAsync(CancellationToken ct = default)
    {
        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        return await MapPagesAsync(db.Results, ct);
    }

    public async Task<FoodLog?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        NotionPage page = await _client.GetPageAsync(pageId, ct);
        return await MapPageAsync(page, ct);
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
        return await MapPageAsync(page, ct);
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default) =>
        await _client.ArchivePageAsync(pageId, ct);

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
}
