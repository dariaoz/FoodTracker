using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Application.Products;

public class ProductService : IProductService
{
    private readonly INotionContext _context;
    private readonly IValidator<Product> _validator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(INotionContext context, IValidator<Product> validator, ILogger<ProductService> logger)
    {
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public Task<IList<Product>> GetAllAsync(CancellationToken ct = default) => _context.Products.GetAllAsync(ct);
    public Task<Product?> GetByIdAsync(string id, CancellationToken ct = default) => _context.Products.GetByIdAsync(id, ct);

    public async Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        _validator.Validate(product);
        Product created = await _context.Products.CreateAsync(product, ct);
        _logger.LogInformation("Created product {Id} {Name}", created.Id, created.Name);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _context.Products.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted product {Id}", id);
    }
}
