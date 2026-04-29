using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Application.Recipes;

public class RecipeService : IRecipeService
{
    private readonly INotionContext _context;
    private readonly ISearchContext _searchContext;
    private readonly IValidator<Recipe> _validator;
    private readonly IIndexingService _indexing;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(
        INotionContext context,
        ISearchContext searchContext,
        IValidator<Recipe> validator,
        IIndexingService indexing,
        ILogger<RecipeService> logger)
    {
        _context = context;
        _searchContext = searchContext;
        _validator = validator;
        _indexing = indexing;
        _logger = logger;
    }

    public Task<IReadOnlyList<Recipe>> SearchAsync(RecipeFilter filter, CancellationToken ct = default) =>
        _searchContext.Recipes.SearchAsync(filter, ct);

    public Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default) =>
        _searchContext.Recipes.GetByIdAsync(id, ct);

    public async Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default)
    {
        _validator.Validate(recipe);
        var created = await _context.Recipes.CreateAsync(recipe, ct);
        _logger.LogInformation("Created recipe {Id} {Name}", created.Id, created.Name);
        await _indexing.SyncIndexAsync(() => _searchContext.Recipes.IndexAsync(created, ct), $"index recipe {created.Id}", ct);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _context.Recipes.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted recipe {Id}", id);
        await _indexing.SyncIndexAsync(() => _searchContext.Recipes.DeleteAsync(id, ct), $"delete recipe {id}", ct);
    }
}
