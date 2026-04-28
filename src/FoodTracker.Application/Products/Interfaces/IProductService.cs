using FoodTracker.Domain.Products;

namespace FoodTracker.Application.Products.Interfaces;

public interface IProductService
{
    Task<IList<Product>> SearchAsync(ProductFilter filter, CancellationToken ct = default);
    Task<Product?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Product> CreateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
