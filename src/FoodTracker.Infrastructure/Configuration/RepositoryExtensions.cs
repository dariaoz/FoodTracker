using FoodTracker.Application.Repositories;
using FoodTracker.Application.UnitOfWork;
using FoodTracker.Infrastructure.Notion.Repositories;
using FoodTracker.Infrastructure.Notion.UnitOfWork;
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
