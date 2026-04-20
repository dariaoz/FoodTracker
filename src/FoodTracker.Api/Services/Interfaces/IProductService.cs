using FoodTracker.Api.Domain.Entities;

namespace FoodTracker.Api.Services;

public interface IProductService
{
    Task<IList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Product> CreateAsync(Product product, CancellationToken ct = default);
    Task<Product> UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
