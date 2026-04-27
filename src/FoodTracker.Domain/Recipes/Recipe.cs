using FoodTracker.Domain.Shared;

namespace FoodTracker.Domain.Recipes;

public class Recipe : IMacroSource, IHaveId
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Servings { get; init; }
    public ServingUnit ServingUnit { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }
}
