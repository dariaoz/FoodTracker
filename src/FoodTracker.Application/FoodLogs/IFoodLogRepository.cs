using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;

namespace FoodTracker.Application.FoodLogs;

public interface IFoodLogRepository : IRepository<FoodLog>
{
    Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}
