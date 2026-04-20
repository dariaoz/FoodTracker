using FoodTracker.Api.Domain.Entities;

namespace FoodTracker.Api.Notion.Repositories;

public interface IFoodLogRepository : IRepository<FoodLog>
{
    Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}
