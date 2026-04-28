using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;

namespace FoodTracker.Infrastructure.Notion;

internal class NotionContext : INotionContext
{
    public IRepository<Product> Products { get; }
    public IRepository<Recipe> Recipes { get; }
    public IRepository<FoodLog> FoodLogs { get; }

    public NotionContext(
        IRepository<Product> products,
        IRepository<Recipe> recipes,
        IRepository<FoodLog> foodLogs)
    {
        Products = products;
        Recipes = recipes;
        FoodLogs = foodLogs;
    }
}
