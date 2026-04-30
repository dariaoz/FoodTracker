using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Application.Shared;

namespace FoodTracker.Infrastructure.Elasticsearch;

internal class SearchContext : ISearchContext
{
    public SearchContext(IFoodLogSearchRepository foodLogs,
        IProductSearchRepository products,
        IRecipeSearchRepository recipes)
    {
        FoodLogs = foodLogs;
        Products = products;
        Recipes = recipes;
    }

    public IFoodLogSearchRepository FoodLogs { get; }
    public IProductSearchRepository Products { get; }
    public IRecipeSearchRepository Recipes { get; }
}
