using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes.Interfaces;

namespace FoodTracker.Application.Shared;

public interface ISearchContext
{
    IFoodLogSearchRepository FoodLogs { get; }
    IProductSearchRepository Products { get; }
    IRecipeSearchRepository Recipes { get; }
}
