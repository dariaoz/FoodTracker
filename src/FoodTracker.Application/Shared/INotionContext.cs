using FoodTracker.Application.Products;
using FoodTracker.Application.Recipes;
using FoodTracker.Application.FoodLogs;

namespace FoodTracker.Application.Shared;

public interface INotionContext
{
    IProductRepository Products { get; }
    IRecipeRepository Recipes { get; }
    IFoodLogRepository FoodLogs { get; }
}
