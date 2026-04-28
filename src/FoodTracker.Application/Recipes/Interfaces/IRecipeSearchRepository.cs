using FoodTracker.Application.Shared;
using FoodTracker.Domain.Recipes;

namespace FoodTracker.Application.Recipes.Interfaces;

public interface IRecipeSearchRepository : ISearchRepository<Recipe>
{
    Task<IList<Recipe>> SearchAsync(RecipeFilter filter, CancellationToken ct = default);
}
