using FoodTracker.Domain.Recipes;

namespace FoodTracker.Api.Models.Recipes;

internal static class RecipeMappings
{
    internal static Recipe ToDomain(this RecipeRequest r, string id = "") => new()
    {
        Id = id,
        Name = r.Name,
        Servings = r.Servings,
        ServingUnit = r.ServingUnit,
        Calories = r.Calories,
        Protein = r.Protein,
        Carbs = r.Carbs,
        Fat = r.Fat
    };

    internal static RecipeResponse ToResponse(this Recipe r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Servings = r.Servings,
        ServingUnit = r.ServingUnit,
        Calories = r.Calories,
        Protein = r.Protein,
        Carbs = r.Carbs,
        Fat = r.Fat
    };
}
