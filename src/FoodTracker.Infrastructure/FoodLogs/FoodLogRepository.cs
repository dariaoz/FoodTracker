using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Notion;
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
        return db.Results.Select(FoodLogNotionMapper.ToEntity).ToList();
    }

    public async Task<FoodLog?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        NotionPage page = await _client.GetPageAsync(pageId, ct);
        return FoodLogNotionMapper.ToEntity(page);
    }

    public async Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        var filter = new
        {
            property = "Date",
            date = new { equals = date.ToString("yyyy-MM-dd") }
        };
        NotionDatabase db = await _client.QueryDatabaseAsync(_databaseId, filter, ct);
        return db.Results.Select(FoodLogNotionMapper.ToEntity).ToList();
    }

    public async Task<FoodLog> CreateAsync(FoodLog entity, CancellationToken ct = default)
    {
        NotionPage page = await _client.CreatePageAsync(_databaseId, FoodLogNotionMapper.ToNotionProperties(entity), ct);
        return FoodLogNotionMapper.ToEntity(page);
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default) =>
        await _client.ArchivePageAsync(_databaseId, pageId, ct);
}
