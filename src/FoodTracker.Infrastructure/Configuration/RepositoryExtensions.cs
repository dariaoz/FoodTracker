using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;
using FoodTracker.Infrastructure.FoodLogs;
using FoodTracker.Infrastructure.Notion;
using FoodTracker.Infrastructure.Products;
using FoodTracker.Infrastructure.Recipes;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure.Configuration;

internal static class RepositoryExtensions
{
    public static void AddNotionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<IRepository<Recipe>, RecipeRepository>();
        services.AddScoped<IRepository<FoodLog>, FoodLogRepository>();
        services.AddScoped<INotionContext, NotionContext>();
    }
}
