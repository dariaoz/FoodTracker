using FoodTracker.Domain.Shared;

namespace FoodTracker.Api.Models.FoodLogs;

public class FoodLogRequest
{
    public DateOnly Date { get; init; }
    public string? RecipeId { get; init; }
    public string? ProductId { get; init; }
    public ServingUnit ServingUnit { get; init; }
    public decimal Quantity { get; init; }
}
