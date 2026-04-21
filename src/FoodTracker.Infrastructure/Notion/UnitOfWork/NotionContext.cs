using FoodTracker.Application.Repositories;
using FoodTracker.Application.UnitOfWork;

namespace FoodTracker.Infrastructure.Notion.UnitOfWork;

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
