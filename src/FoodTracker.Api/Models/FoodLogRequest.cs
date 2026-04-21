using FoodTracker.Domain.Entities;

namespace FoodTracker.Api.Models;

public class FoodLogRequest
{
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
}
