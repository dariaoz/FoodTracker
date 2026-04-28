using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Infrastructure.Elasticsearch;

internal abstract class ElasticsearchRepositoryBase<T> : ISearchRepository<T> where T : IHaveId
{
    private const int MaxResults = 10000;

    protected readonly ElasticsearchClient Client;
    protected readonly string Index;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    protected ElasticsearchRepositoryBase(
        ElasticsearchClient client,
        string index,
        IServiceScopeFactory scopeFactory,
        ILogger logger)
    {
        Client = client;
        Index = index;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var response = await Client.GetAsync<T>(Index, id, ct);
        EnsureValid(response, nameof(GetByIdAsync));
        return response.Found ? response.Source : default;
    }

    protected async Task<IList<T>> GetAllAsync(CancellationToken ct = default)
    {
        var response = await Client.SearchAsync<T>(s => s
            .Indices(Index)
            .Size(MaxResults)
            .Query(q => q.MatchAll()), ct);
        EnsureValid(response, nameof(GetAllAsync));
        return response.Documents.ToList();
    }

    protected async Task<IList<T>> SearchByNameAsync(string? name, Expression<Func<T, string>> nameField, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetAllAsync(ct);

        var response = await Client.SearchAsync<T>(s => s
            .Indices(Index)
            .Size(MaxResults)
            .Query(q => q.Match(m => m
                .Field(nameField)
                .Query(name)
                .Fuzziness(new Fuzziness("auto")))), ct);

        EnsureValid(response, nameof(SearchByNameAsync));
        return response.Documents.ToList();
    }

    public async Task IndexAsync(T entity, CancellationToken ct = default)
    {
        _logger.LogDebug("Indexing {Type} {Id} to ES index {Index}", typeof(T).Name, entity.Id, Index);
        var response = await Client.IndexAsync(entity, i => i.Index(Index).Id(entity.Id), ct);
        EnsureValid(response, nameof(IndexAsync));
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        _logger.LogDebug("Deleting {Type} {Id} from ES index {Index}", typeof(T).Name, id, Index);
        var response = await Client.DeleteAsync(Index, id, ct);
        EnsureValid(response, nameof(DeleteAsync));
    }

    private void EnsureValid(ElasticsearchResponse response, string operation)
    {
        if (response.IsValidResponse)
            return;

        _logger.LogError(
            "ES {Operation} failed on index {Index}. Reason: {Reason}. Debug: {DebugInfo}",
            operation, Index,
            response.ElasticsearchServerError?.Error?.Reason ?? "unknown",
            response.DebugInformation);

        throw new InvalidOperationException(
            $"Elasticsearch {operation} on index '{Index}' failed: {response.ElasticsearchServerError?.Error?.Reason ?? response.DebugInformation}");
    }

    protected async Task ReindexFromAsync(Func<IServiceProvider, IRepository<T>> factory, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = factory(scope.ServiceProvider);
        var entities = await repo.GetAllAsync(ct);
        foreach (var entity in entities)
            await IndexAsync(entity, ct);
    }
}
