using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Application.Recipes;

public class RecipeService : IRecipeService
{
    private readonly INotionContext _context;
    private readonly IValidator<Recipe> _validator;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(INotionContext context, IValidator<Recipe> validator, ILogger<RecipeService> logger)
    {
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default) => _context.Recipes.GetAllAsync(ct);
    public Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default) => _context.Recipes.GetByIdAsync(id, ct);

    public async Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default)
    {
        _validator.Validate(recipe);
        Recipe created = await _context.Recipes.CreateAsync(recipe, ct);
        _logger.LogInformation("Created recipe {Id} {Name}", created.Id, created.Name);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _context.Recipes.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted recipe {Id}", id);
    }
}
