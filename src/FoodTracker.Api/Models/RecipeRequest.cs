namespace FoodTracker.Api.Models;

public class RecipeRequest
{
    public string Name { get; init; } = string.Empty;
    public int Servings { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }
}
