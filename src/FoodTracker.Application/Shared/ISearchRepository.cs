namespace FoodTracker.Application.Shared;

public interface ISearchRepository<T>
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task IndexAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
