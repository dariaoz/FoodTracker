using FoodTracker.Domain.Recipes;

namespace FoodTracker.Application.Recipes.Interfaces;

public interface IRecipeService
{
    Task<IReadOnlyList<Recipe>> SearchAsync(RecipeFilter filter, CancellationToken ct = default);
    Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
