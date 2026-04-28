using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;

namespace FoodTracker.Application.Shared;

public interface INotionContext
{
    IRepository<Product> Products { get; }
    IRepository<Recipe> Recipes { get; }
    IRepository<FoodLog> FoodLogs { get; }
}
