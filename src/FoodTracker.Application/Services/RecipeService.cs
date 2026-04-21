using FoodTracker.Application.Services.Interfaces;
using FoodTracker.Domain.Entities;
using FoodTracker.Domain.Validation;
using FoodTracker.Application.UnitOfWork;

namespace FoodTracker.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly INotionUnitOfWork _uow;
    private readonly IValidator<Recipe> _validator;

    public RecipeService(INotionUnitOfWork uow, IValidator<Recipe> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default) => _uow.Recipes.GetAllAsync(ct);
    public Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default) => _uow.Recipes.GetByIdAsync(id, ct);

    public Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default)
    {
        _validator.Validate(recipe);
        return _uow.Recipes.CreateAsync(recipe, ct);
    }

    public Task<Recipe> UpdateAsync(Recipe recipe, CancellationToken ct = default)
    {
        _validator.Validate(recipe);
        return _uow.Recipes.UpdateAsync(recipe, ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) => _uow.Recipes.DeleteAsync(id, ct);
}
