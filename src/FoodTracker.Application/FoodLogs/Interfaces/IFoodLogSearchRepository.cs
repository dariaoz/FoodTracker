using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;

namespace FoodTracker.Application.FoodLogs.Interfaces;

public interface IFoodLogSearchRepository : ISearchRepository<FoodLog>
{
    Task<IReadOnlyList<FoodLog>> SearchAsync(FoodLogFilter filter, CancellationToken ct = default);
}
