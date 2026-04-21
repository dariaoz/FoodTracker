using FoodTracker.Domain.Recipes;

namespace FoodTracker.Application.Recipes;

public interface IRecipeService
{
    Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default);
    Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
