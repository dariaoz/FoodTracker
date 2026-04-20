using FoodTracker.Api.Domain.Interfaces;

namespace FoodTracker.Api.Domain.Entities;

public class Recipe : IHaveMacronutrients
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Servings { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }

    public Serving Serving => new(ServingUnit.Portion, 1m);
}
