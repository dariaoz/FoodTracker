using FoodTracker.Domain.Interfaces;

namespace FoodTracker.Domain.Entities;

public class FoodLog : IHaveMacronutrients
{
    public string Id { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public string? RecipeId { get; init; }
    public string? ProductId { get; init; }
    public ServingUnit ServingUnit { get; init; }
    public decimal Quantity { get; init; }
    public decimal PortionQ { get; init; }

    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }

    public Serving Serving => new(ServingUnit, PortionQ);
}
