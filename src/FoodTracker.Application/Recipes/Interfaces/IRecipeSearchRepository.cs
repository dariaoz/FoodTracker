using FoodTracker.Application.Shared;
using FoodTracker.Domain.Recipes;

namespace FoodTracker.Application.Recipes.Interfaces;

public interface IRecipeSearchRepository : ISearchRepository<Recipe>
{
    Task<IReadOnlyList<Recipe>> SearchAsync(RecipeFilter filter, CancellationToken ct = default);
}
