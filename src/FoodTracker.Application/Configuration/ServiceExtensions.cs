using FoodTracker.Domain.Entities;
using FoodTracker.Domain.Validation;
using FoodTracker.Application.Services;
using FoodTracker.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Application.Configuration;

internal static class ServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<Product>, ProductValidator>();
        services.AddSingleton<IValidator<Recipe>, RecipeValidator>();
        services.AddSingleton<IValidator<FoodLog>, FoodLogValidator>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IFoodLogService, FoodLogService>();
    }
}
