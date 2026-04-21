using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.Products;
using FoodTracker.Application.Recipes;
using FoodTracker.Application.Shared;

namespace FoodTracker.Infrastructure.Shared;

internal class NotionContext : INotionContext
{
    public IProductRepository Products { get; }
    public IRecipeRepository Recipes { get; }
    public IFoodLogRepository FoodLogs { get; }

    public NotionContext(
        IProductRepository products,
        IRecipeRepository recipes,
        IFoodLogRepository foodLogs)
    {
        Products = products;
        Recipes = recipes;
        FoodLogs = foodLogs;
    }
}
