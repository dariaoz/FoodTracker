using FoodTracker.Application.Repositories;

namespace FoodTracker.Application.UnitOfWork;

public interface INotionContext
{
    IProductRepository Products { get; }
    IRecipeRepository Recipes { get; }
    IFoodLogRepository FoodLogs { get; }
}
