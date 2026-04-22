using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes.Interfaces;

namespace FoodTracker.Application.Shared;

public interface INotionContext
{
    IProductRepository Products { get; }
    IRecipeRepository Recipes { get; }
    IFoodLogRepository FoodLogs { get; }
}
