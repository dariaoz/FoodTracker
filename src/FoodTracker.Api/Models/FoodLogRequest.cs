using FoodTracker.Domain.Entities;

namespace FoodTracker.Api.Models;

public class FoodLogRequest
{
    public DateOnly Date { get; init; }
    public string? RecipeId { get; init; }
    public string? ProductId { get; init; }
    public ServingUnit ServingUnit { get; init; }
    public decimal Quantity { get; init; }
}
