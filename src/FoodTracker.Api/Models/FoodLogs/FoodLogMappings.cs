using FoodTracker.Domain.FoodLogs;

namespace FoodTracker.Api.Models.FoodLogs;

internal static class FoodLogMappings
{
    internal static FoodLog ToDomain(this FoodLogRequest r, string id = "") => new()
    {
        Id = id,
        Date = r.Date,
        RecipeId = r.RecipeId,
        ProductId = r.ProductId,
        ServingUnit = r.ServingUnit,
        Quantity = r.Quantity
    };

    internal static FoodLogResponse ToResponse(this FoodLog l) => new()
    {
        Id = l.Id,
        Date = l.Date,
        RecipeId = l.RecipeId,
        ProductId = l.ProductId,
        ServingUnit = l.ServingUnit,
        Quantity = l.Quantity,
        Calories = l.Calories,
        Protein = l.Protein,
        Carbs = l.Carbs,
        Fat = l.Fat
    };
}
