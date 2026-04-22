using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;

namespace FoodTracker.Application.FoodLogs.Interfaces;

public interface IFoodLogRepository : IRepository<FoodLog>
{
    Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}
