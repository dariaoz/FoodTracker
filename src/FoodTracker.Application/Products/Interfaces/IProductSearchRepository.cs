using FoodTracker.Application.Shared;
using FoodTracker.Domain.Products;

namespace FoodTracker.Application.Products.Interfaces;

public interface IProductSearchRepository : ISearchRepository<Product>
{
    Task<IReadOnlyList<Product>> SearchAsync(ProductFilter filter, CancellationToken ct = default);
}
