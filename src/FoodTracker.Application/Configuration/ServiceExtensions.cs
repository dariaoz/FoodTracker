using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Products;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes;
using FoodTracker.Application.Recipes.Interfaces;
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
