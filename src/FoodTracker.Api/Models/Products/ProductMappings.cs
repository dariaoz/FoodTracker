using FoodTracker.Domain.Products;

namespace FoodTracker.Api.Models.Products;

internal static class ProductMappings
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
}
