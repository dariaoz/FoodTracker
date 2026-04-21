using FoodTracker.Application.Repositories;

namespace FoodTracker.Application.UnitOfWork;

public interface INotionUnitOfWork
{
    IProductRepository Products { get; }
    IRecipeRepository Recipes { get; }
    IFoodLogRepository FoodLogs { get; }
}
