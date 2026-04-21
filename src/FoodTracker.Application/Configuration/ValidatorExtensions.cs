using FoodTracker.Domain.Entities;
using FoodTracker.Domain.Validation;
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
