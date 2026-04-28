using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Application.Products;

public class ProductService : IProductService
{
    private readonly INotionContext _context;
    private readonly ISearchContext _searchContext;
    private readonly IValidator<Product> _validator;
    private readonly IIndexingService _indexing;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        INotionContext context,
        ISearchContext searchContext,
        IValidator<Product> validator,
        IIndexingService indexing,
        ILogger<ProductService> logger)
    {
        _context = context;
        _searchContext = searchContext;
        _validator = validator;
        _indexing = indexing;
        _logger = logger;
    }

    public Task<IList<Product>> SearchAsync(ProductFilter filter, CancellationToken ct = default) =>
        _searchContext.Products.SearchAsync(filter, ct);

    public Task<Product?> GetByIdAsync(string id, CancellationToken ct = default) =>
        _searchContext.Products.GetByIdAsync(id, ct);

    public async Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        _validator.Validate(product);
        var created = await _context.Products.CreateAsync(product, ct);
        _logger.LogInformation("Created product {Id} {Name}", created.Id, created.Name);
        await _indexing.SyncAsync(() => _searchContext.Products.IndexAsync(created, ct), $"index product {created.Id}", ct);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _context.Products.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted product {Id}", id);
        await _indexing.SyncAsync(() => _searchContext.Products.DeleteAsync(id, ct), $"delete product {id}", ct);
    }
}
