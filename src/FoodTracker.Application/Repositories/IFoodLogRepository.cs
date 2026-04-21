using FoodTracker.Domain.Entities;

namespace FoodTracker.Application.Repositories;

public interface IFoodLogRepository : IRepository<FoodLog>
{
    Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}
