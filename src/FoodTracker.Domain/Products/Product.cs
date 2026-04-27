using FoodTracker.Domain.Shared;

namespace FoodTracker.Domain.Products;

public class Product : IMacroSource, IHaveId
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ServingUnit ServingUnit { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }
}
