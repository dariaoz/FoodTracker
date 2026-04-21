namespace FoodTracker.Application.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(string pageId, CancellationToken ct = default);
    Task<IList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> CreateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string pageId, CancellationToken ct = default);
}
