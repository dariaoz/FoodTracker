using FoodTracker.Api.Domain.Entities;

namespace FoodTracker.Api.Services;

public interface IRecipeService
{
    Task<IList<Recipe>> GetAllAsync(CancellationToken ct = default);
    Task<Recipe?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Recipe> CreateAsync(Recipe recipe, CancellationToken ct = default);
    Task<Recipe> UpdateAsync(Recipe recipe, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
