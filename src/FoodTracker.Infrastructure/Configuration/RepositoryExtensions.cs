using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.Products;
using FoodTracker.Application.Recipes;
using FoodTracker.Application.Shared;
using FoodTracker.Infrastructure.FoodLogs;
using FoodTracker.Infrastructure.Products;
using FoodTracker.Infrastructure.Recipes;
using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure.Configuration;

internal static class RepositoryExtensions
{
    public static void AddNotionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogRepository>();

        services.Decorate<IProductRepository, CachedProductRepository>();
        services.Decorate<IRecipeRepository, CachedRecipeRepository>();
        services.Decorate<IFoodLogRepository, CachedFoodLogRepository>();

        services.AddScoped<INotionContext, NotionContext>();
    }
}
