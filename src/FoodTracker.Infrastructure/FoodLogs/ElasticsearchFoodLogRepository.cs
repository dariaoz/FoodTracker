using Elastic.Clients.Elasticsearch;
using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Infrastructure.Elasticsearch;
using FoodTracker.Infrastructure.Elasticsearch.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.FoodLogs;

internal class ElasticsearchFoodLogRepository : ElasticsearchRepositoryBase<FoodLog>, IFoodLogSearchRepository, IReindexable
{
    public ElasticsearchFoodLogRepository(
        ElasticsearchClient client,
        IOptions<ElasticsearchOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<ElasticsearchFoodLogRepository> logger)
        : base(client, options.Value.FoodLogsIndex, scopeFactory, logger)
    {
    }
    public async Task<IList<FoodLog>> SearchAsync(FoodLogFilter filter, CancellationToken ct = default)
    {
        if (!filter.DateFrom.HasValue && !filter.DateTo.HasValue)
            return await GetAllAsync(ct);

        var response = await Client.SearchAsync<FoodLog>(s => s
            .Indices(Index)
            .Size(10000)
            .Query(q => q.Range(r => r.Date(d =>
            {
                d.Field(f => f.Date);
                if (filter.DateFrom.HasValue) d.Gte(filter.DateFrom.Value.ToString("yyyy-MM-dd"));
                if (filter.DateTo.HasValue) d.Lte(filter.DateTo.Value.ToString("yyyy-MM-dd"));
            }))), ct);

        return response.Documents.ToList();
    }

    public Task ReindexAsync(CancellationToken ct) =>
        ReindexFromAsync(sp => sp.GetRequiredService<IRepository<FoodLog>>(), ct);
}
