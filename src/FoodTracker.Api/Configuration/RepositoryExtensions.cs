using FoodTracker.Api.Notion.Repositories;
using FoodTracker.Api.Notion.UnitOfWork;

namespace FoodTracker.Api.Configuration;

public static class RepositoryExtensions
{
    public static IServiceCollection AddNotionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogRepository>();
        services.AddScoped<INotionUnitOfWork, NotionUnitOfWork>();

        return services;
    }
}