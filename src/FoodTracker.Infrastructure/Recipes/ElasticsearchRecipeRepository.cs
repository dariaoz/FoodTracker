using Elastic.Clients.Elasticsearch;
using FoodTracker.Application.Recipes;
using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Recipes;
using FoodTracker.Infrastructure.Elasticsearch;
using FoodTracker.Infrastructure.Elasticsearch.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Recipes;

internal class ElasticsearchRecipeRepository : ElasticsearchRepositoryBase<Recipe>, IRecipeSearchRepository, IReindexable
{
    public ElasticsearchRecipeRepository(
        ElasticsearchClient client,
        IOptions<ElasticsearchOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<ElasticsearchRecipeRepository> logger)
        : base(client, options.Value.RecipesIndex, scopeFactory, logger)
    {
    }
    public Task<IList<Recipe>> SearchAsync(RecipeFilter filter, CancellationToken ct = default) =>
        SearchByNameAsync(filter.Name, f => f.Name, ct);

    public Task ReindexAsync(CancellationToken ct) =>
        ReindexFromAsync(sp => sp.GetRequiredService<IRepository<Recipe>>(), ct);
}
