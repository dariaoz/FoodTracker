namespace FoodTracker.Application.FoodLogs;

public record FoodLogFilter(DateOnly? DateFrom = null, DateOnly? DateTo = null);
