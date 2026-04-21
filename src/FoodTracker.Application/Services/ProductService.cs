using FoodTracker.Application.Services.Interfaces;
using FoodTracker.Domain.Entities;
using FoodTracker.Domain.Validation;
using FoodTracker.Application.UnitOfWork;

namespace FoodTracker.Application.Services;

public class ProductService : IProductService
{
    private readonly INotionContext _context;
    private readonly IValidator<Product> _validator;

    public ProductService(INotionContext context, IValidator<Product> validator)
    {
        _context = context;
        _validator = validator;
    }

    public Task<IList<Product>> GetAllAsync(CancellationToken ct = default) => _context.Products.GetAllAsync(ct);
    public Task<Product?> GetByIdAsync(string id, CancellationToken ct = default) => _context.Products.GetByIdAsync(id, ct);

    public Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        _validator.Validate(product);
        return _context.Products.CreateAsync(product, ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) => _context.Products.DeleteAsync(id, ct);
}
