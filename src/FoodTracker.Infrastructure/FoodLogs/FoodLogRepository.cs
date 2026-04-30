using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Infrastructure.Notion;
using FoodTracker.Infrastructure.Notion.Configuration;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.FoodLogs;

internal class FoodLogRepository : IRepository<FoodLog>
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
        var db = await _client.QueryDatabaseAsync(_databaseId, ct: ct);
        return db.Results.Select(FoodLogNotionMapper.ToEntity).ToList();
    }

    public async Task<FoodLog?> GetByIdAsync(string pageId, CancellationToken ct = default)
    {
        var page = await _client.GetPageAsync(pageId, ct);
        return FoodLogNotionMapper.ToEntity(page);
    }

    public async Task<FoodLog> CreateAsync(FoodLog entity, CancellationToken ct = default)
    {
        var page = await _client.CreatePageAsync(_databaseId, FoodLogNotionMapper.ToNotionProperties(entity), ct);
        return FoodLogNotionMapper.ToEntity(page);
    }

    public async Task DeleteAsync(string pageId, CancellationToken ct = default) =>
        await _client.DeletePageAsync(_databaseId, pageId, ct);
}
