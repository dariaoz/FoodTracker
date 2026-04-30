using Elastic.Clients.Elasticsearch;
using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Infrastructure.FoodLogs;
using FoodTracker.Infrastructure.Products;
using FoodTracker.Infrastructure.Recipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Elasticsearch.Configuration;

internal static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticsearchOptions>(configuration.GetSection(ElasticsearchOptions.SectionName));

        services.AddSingleton<ElasticsearchClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            var settings = new ElasticsearchClientSettings(new Uri(options.Url));
            return new ElasticsearchClient(settings);
        });

        services.AddElasticsearchRepository<ElasticsearchProductRepository, IProductSearchRepository>();
        services.AddElasticsearchRepository<ElasticsearchRecipeRepository, IRecipeSearchRepository>();
        services.AddElasticsearchRepository<ElasticsearchFoodLogRepository, IFoodLogSearchRepository>();

        services.AddSingleton<ISearchContext, SearchContext>();
    }

    private static void AddElasticsearchRepository<TRepo, TSearchRepo>(this IServiceCollection services)
        where TRepo : class, TSearchRepo, IReindexable
        where TSearchRepo : class
    {
        services.AddSingleton<TRepo>();
        services.AddSingleton<TSearchRepo>(sp => sp.GetRequiredService<TRepo>());
        services.AddSingleton<IReindexable>(sp => sp.GetRequiredService<TRepo>());
    }
}
