using FoodTracker.Api.Domain.Entities;
using FoodTracker.Api.Domain.Validation;
using FoodTracker.Api.Services;

namespace FoodTracker.Api.Configuration;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<Product>, ProductValidator>();
        services.AddSingleton<IValidator<Recipe>, RecipeValidator>();
        services.AddSingleton<IValidator<FoodLog>, FoodLogValidator>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IFoodLogService, FoodLogService>();

        return services;
    }
}