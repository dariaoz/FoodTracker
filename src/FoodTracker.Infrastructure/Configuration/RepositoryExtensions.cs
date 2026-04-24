using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;
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
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogRepository>();

        services.Decorate<IProductRepository, CachedProductRepository>();
        services.Decorate<IRecipeRepository, CachedRecipeRepository>();
        services.Decorate<IFoodLogRepository, CachedFoodLogRepository>();

        services.AddScoped<INotionContext, NotionContext>();
    }
}
