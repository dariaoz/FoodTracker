using FoodTracker.Application.Services.Interfaces;
using FoodTracker.Domain.Entities;
using FoodTracker.Domain.Validation;
using FoodTracker.Application.UnitOfWork;

namespace FoodTracker.Application.Services;

public class ProductService : IProductService
{
    private readonly INotionUnitOfWork _uow;
    private readonly IValidator<Product> _validator;

    public ProductService(INotionUnitOfWork uow, IValidator<Product> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public Task<IList<Product>> GetAllAsync(CancellationToken ct = default) => _uow.Products.GetAllAsync(ct);
    public Task<Product?> GetByIdAsync(string id, CancellationToken ct = default) => _uow.Products.GetByIdAsync(id, ct);

    public Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        _validator.Validate(product);
        return _uow.Products.CreateAsync(product, ct);
    }

    public Task<Product> UpdateAsync(Product product, CancellationToken ct = default)
    {
        _validator.Validate(product);
        return _uow.Products.UpdateAsync(product, ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) => _uow.Products.DeleteAsync(id, ct);
}
