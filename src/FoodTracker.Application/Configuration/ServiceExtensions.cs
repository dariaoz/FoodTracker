using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.Products;
using FoodTracker.Application.Recipes;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Application.Configuration;

internal static class ServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IFoodLogService, FoodLogService>();
    }
}
