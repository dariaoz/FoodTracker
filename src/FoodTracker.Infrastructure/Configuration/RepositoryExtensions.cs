using FoodTracker.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;
using FoodTracker.Application.UnitOfWork;
using FoodTracker.Infrastructure.Notion.Repositories;
using FoodTracker.Infrastructure.Notion.UnitOfWork;

namespace FoodTracker.Infrastructure.Configuration;

internal static class RepositoryExtensions
{
    public static void AddNotionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogRepository>();
        services.AddScoped<INotionUnitOfWork, NotionUnitOfWork>();
    }
}
