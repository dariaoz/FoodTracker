using FoodTracker.Domain.FoodLogs;

namespace FoodTracker.Application.FoodLogs.Interfaces;

public interface IFoodLogService
{
    Task<IReadOnlyList<FoodLog>> GetAsync(FoodLogFilter filter, CancellationToken ct = default);
    Task<FoodLog?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<FoodLog> CreateAsync(FoodLog foodLog, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
