using FoodTracker.Domain.Entities;

namespace FoodTracker.Api.Models;

internal static class Mappings
{
    internal static Product ToDomain(this ProductRequest r, string id = "") => new()
    {
        Id = id,
        Name = r.Name,
        ServingUnit = r.ServingUnit,
        Calories = r.Calories,
        Protein = r.Protein,
        Carbs = r.Carbs,
        Fat = r.Fat
    };

    internal static ProductResponse ToResponse(this Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        ServingUnit = p.ServingUnit,
        Calories = p.Calories,
        Protein = p.Protein,
        Carbs = p.Carbs,
        Fat = p.Fat
    };

    internal static Recipe ToDomain(this RecipeRequest r, string id = "") => new()
    {
        Id = id,
        Name = r.Name,
        Servings = r.Servings,
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
        Calories = r.Calories,
        Protein = r.Protein,
        Carbs = r.Carbs,
        Fat = r.Fat
    };

    internal static FoodLog ToDomain(this FoodLogRequest r, string id = "") => new()
    {
        Id = id,
        Date = r.Date,
        RecipeId = r.RecipeId,
        ProductId = r.ProductId,
        ServingUnit = r.ServingUnit,
        Quantity = r.Quantity,
        PortionQ = r.PortionQ,
        Calories = r.Calories,
        Protein = r.Protein,
        Carbs = r.Carbs,
        Fat = r.Fat
    };

    internal static FoodLogResponse ToResponse(this FoodLog l) => new()
    {
        Id = l.Id,
        Date = l.Date,
        RecipeId = l.RecipeId,
        ProductId = l.ProductId,
        ServingUnit = l.ServingUnit,
        Quantity = l.Quantity,
        PortionQ = l.PortionQ,
        Calories = l.Calories,
        Protein = l.Protein,
        Carbs = l.Carbs,
        Fat = l.Fat
    };
}
