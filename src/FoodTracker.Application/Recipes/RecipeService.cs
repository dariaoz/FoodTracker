using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;

namespace FoodTracker.Application.Recipes;

public class RecipeService : IRecipeService
{
    private readonly INotionContext _context;
    private readonly IValidator<Recipe> _validator;

    public RecipeService(INotionContext context, IValidator<Recipe> validator)
    {
        _context = context;
        _validator = validator;
    }

    public Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default) => _context.Recipes.GetAllAsync(ct);
    public Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default) => _context.Recipes.GetByIdAsync(id, ct);

    public Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default)
    {
        _validator.Validate(recipe);
        return _context.Recipes.CreateAsync(recipe, ct);
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) => _context.Recipes.DeleteAsync(id, ct);
}
