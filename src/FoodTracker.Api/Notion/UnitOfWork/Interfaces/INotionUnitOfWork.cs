using FoodTracker.Api.Notion.Repositories;

namespace FoodTracker.Api.Notion.UnitOfWork;

public interface INotionUnitOfWork
{
    IProductRepository Products { get; }
    IRecipeRepository Recipes { get; }
    IFoodLogRepository FoodLogs { get; }
}
