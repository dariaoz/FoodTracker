using FoodTracker.Domain.Shared;

namespace FoodTracker.Api.Models.Products;

public class ProductRequest
{
    public string Name { get; init; } = string.Empty;
    public ServingUnit ServingUnit { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }
}
