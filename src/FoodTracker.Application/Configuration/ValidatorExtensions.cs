using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Application.Configuration;

internal static class ValidatorExtensions
{
    public static void AddAppValidators(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<Product>, ProductValidator>();
        services.AddSingleton<IValidator<Recipe>, RecipeValidator>();
        services.AddSingleton<IValidator<FoodLog>, FoodLogValidator>();
    }
}
