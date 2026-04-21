using FoodTracker.Application.Repositories;
using FoodTracker.Application.UnitOfWork;

namespace FoodTracker.Infrastructure.Notion.UnitOfWork;

internal class NotionUnitOfWork : INotionUnitOfWork
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
