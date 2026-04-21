using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Notion.Client;

namespace FoodTracker.Infrastructure.Notion.Mappers;

internal static class FoodLogNotionMapper
{
    public static FoodLog ToEntity(NotionPage page, Product? product, Recipe? recipe)
    {
        Dictionary<string, NotionPropertyValue> p = page.Properties;
        decimal portionQ = NotionPropertyHelper.GetDecimal(p, "PortionQ");
        decimal quantity = NotionPropertyHelper.GetDecimal(p, "Quantity");

        double calories, protein, carbs, fat;
        ServingUnit servingUnit;

        if (recipe is not null)
        {
            var factor = (double)portionQ;
            calories = recipe.Calories * factor;
            protein = recipe.Protein * factor;
            carbs = recipe.Carbs * factor;
            fat = recipe.Fat * factor;
            servingUnit = ServingUnit.Portion;
        }
        else if (product is not null)
        {
            var factor = (double)(quantity / 100m);
            calories = product.Calories * factor;
            protein = product.Protein * factor;
            carbs = product.Carbs * factor;
            fat = product.Fat * factor;
            servingUnit = product.ServingUnit;
        }
        else
        {
            calories = protein = carbs = fat = 0;
            servingUnit = ServingUnit.Gram;
        }

        return new FoodLog
        {
            Id = page.Id,
            Date = NotionPropertyHelper.GetDate(p, "Date"),
            RecipeId = NotionPropertyHelper.GetString(p, "RecipeId"),
            ProductId = NotionPropertyHelper.GetString(p, "ProductId"),
            ServingUnit = servingUnit,
            Quantity = quantity,
            PortionQ = portionQ,
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

    public static object ToNotionProperties(FoodLog log) => new
    {
        Date = DateProperty(log.Date),
        RecipeId = RichTextProperty(log.RecipeId ?? string.Empty),
        ProductId = RichTextProperty(log.ProductId ?? string.Empty),
        Quantity = NumberProperty((double)log.Quantity),
        PortionQ = NumberProperty((double)log.PortionQ)
    };

    private static object DateProperty(DateOnly date) => new { date = new { start = date.ToString("yyyy-MM-dd") } };

    private static object RichTextProperty(string value) =>
        new { rich_text = new[] { new { text = new { content = value } } } };

    private static object NumberProperty(double value) => new { number = value };
}
