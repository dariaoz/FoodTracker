using Elastic.Clients.Elasticsearch;
using FoodTracker.Application.Products;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Products;
using FoodTracker.Infrastructure.Elasticsearch;
using FoodTracker.Infrastructure.Elasticsearch.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Products;

internal class ElasticsearchProductRepository : ElasticsearchRepositoryBase<Product>, IProductSearchRepository, IReindexable
{
    public ElasticsearchProductRepository(
        ElasticsearchClient client,
        IOptions<ElasticsearchOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<ElasticsearchProductRepository> logger)
        : base(client, options.Value.ProductsIndex, scopeFactory, logger)
    {
    }
    public Task<IReadOnlyList<Product>> SearchAsync(ProductFilter filter, CancellationToken ct = default) =>
        SearchByNameAsync(filter.Name, f => f.Name, ct);

    public Task ReindexAsync(CancellationToken ct) =>
        ReindexFromAsync(sp => sp.GetRequiredService<IRepository<Product>>(), ct);
}
