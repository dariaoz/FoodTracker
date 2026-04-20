using FoodTracker.Api.Domain.Entities;

namespace FoodTracker.Api.Services;

public interface IFoodLogService
{
    Task<IList<FoodLog>> GetAllAsync(CancellationToken ct = default);
    Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<FoodLog?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<FoodLog> CreateAsync(FoodLog log, CancellationToken ct = default);
    Task<FoodLog> UpdateAsync(FoodLog log, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
