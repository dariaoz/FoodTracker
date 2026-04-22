using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;
using FoodTracker.Infrastructure.Shared;

namespace FoodTracker.Infrastructure.FoodLogs;

internal static class FoodLogNotionMapper
{
    public static FoodLog ToEntity(NotionPage page, Product? product, Recipe? recipe)
    {
        Dictionary<string, NotionPropertyValue> p = page.Properties;
        double quantity = NotionPropertyHelper.GetDouble(p, "Quantity");
        ServingUnit servingUnit = NotionPropertyHelper.GetEnum<ServingUnit>(p, "ServingUnit");

        double calories, protein, carbs, fat;

        if (recipe is not null)
        {
            var factor = quantity / recipe.Serving.Quantity;
            calories = recipe.Calories * factor;
            protein = recipe.Protein * factor;
            carbs = recipe.Carbs * factor;
            fat = recipe.Fat * factor;
        }
        else if (product is not null)
        {
            var factor = quantity / product.Serving.Quantity;
            calories = product.Calories * factor;
            protein = product.Protein * factor;
            carbs = product.Carbs * factor;
            fat = product.Fat * factor;
        }
        else
        {
            calories = protein = carbs = fat = 0;
        }

        return new FoodLog
        {
            Id = page.Id,
            Date = NotionPropertyHelper.GetDate(p, "Date"),
            RecipeId = NotionPropertyHelper.GetString(p, "RecipeId"),
            ProductId = NotionPropertyHelper.GetString(p, "ProductId"),
            ServingUnit = servingUnit,
            Quantity = quantity,
            Calories = calories,
            Protein = protein,
            Carbs = carbs,
            Fat = fat
        };
    }

    public static string GetLinkedRecipeId(Dictionary<string, NotionPropertyValue> props) =>
        NotionPropertyHelper.GetString(props, "RecipeId");

    public static string GetLinkedProductId(Dictionary<string, NotionPropertyValue> props) =>
        NotionPropertyHelper.GetString(props, "ProductId");

    public static object ToNotionProperties(FoodLog foodLog) => new
    {
        Date = DateProperty(foodLog.Date),
        RecipeId = RichTextProperty(foodLog.RecipeId ?? string.Empty),
        ProductId = RichTextProperty(foodLog.ProductId ?? string.Empty),
        ServingUnit = SelectProperty(foodLog.ServingUnit.ToString()),
        Quantity = NumberProperty(foodLog.Quantity)
    };

    private static object DateProperty(DateOnly date) => new { date = new { start = date.ToString("yyyy-MM-dd") } };

    private static object RichTextProperty(string value) =>
        new { rich_text = new[] { new { text = new { content = value } } } };

    private static object NumberProperty(double value) => new { number = value };

    private static object SelectProperty(string value) => new { select = new { name = value } };
}
