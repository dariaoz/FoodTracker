using FoodTracker.Api.Notion.Repositories;

namespace FoodTracker.Api.Notion.UnitOfWork;

public class NotionUnitOfWork : INotionUnitOfWork
{
    public IProductRepository Products { get; }
    public IRecipeRepository Recipes { get; }
    public IFoodLogRepository FoodLogs { get; }

    public NotionUnitOfWork(
        IProductRepository products,
        IRecipeRepository recipes,
        IFoodLogRepository foodLogs)
    {
        Products = products;
        Recipes = recipes;
        FoodLogs = foodLogs;
    }
}
