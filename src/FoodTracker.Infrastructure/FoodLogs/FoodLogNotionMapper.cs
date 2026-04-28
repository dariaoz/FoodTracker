using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Shared;
using FoodTracker.Infrastructure.Notion;

namespace FoodTracker.Infrastructure.FoodLogs;

internal static class FoodLogNotionMapper
{
    public static FoodLog ToEntity(NotionPage page)
    {
        Dictionary<string, NotionPropertyValue> p = page.Properties;
        return new FoodLog
        {
            Id = page.Id,
            Date = NotionPropertyHelper.GetDate(p, "Date"),
            RecipeId = NullIfEmpty(NotionPropertyHelper.GetRelation(p, "Recipe")),
            ProductId = NullIfEmpty(NotionPropertyHelper.GetRelation(p, "Product")),
            ServingUnit = NotionPropertyHelper.GetEnum<ServingUnit>(p, "Serving Unit"),
            Quantity = NotionPropertyHelper.GetDouble(p, "Serving"),
            Calories = NotionPropertyHelper.GetDouble(p, "Calories"),
            Protein = NotionPropertyHelper.GetDouble(p, "Proteins (g)"),
            Carbs = NotionPropertyHelper.GetDouble(p, "Carbonates (g)"),
            Fat = NotionPropertyHelper.GetDouble(p, "Fat (g)")
        };
    }

    public static string GetLinkedRecipeId(Dictionary<string, NotionPropertyValue> props) =>
        NotionPropertyHelper.GetRelation(props, "Recipe");

    public static string GetLinkedProductId(Dictionary<string, NotionPropertyValue> props) =>
        NotionPropertyHelper.GetRelation(props, "Product");

    public static Dictionary<string, object> ToNotionProperties(FoodLog foodLog) => new()
    {
        ["Date"] = DateProperty(foodLog.Date),
        ["Recipe"] = RelationProperty(foodLog.RecipeId),
        ["Product"] = RelationProperty(foodLog.ProductId),
        ["Serving Unit"] = SelectProperty(foodLog.ServingUnit.ToString()),
        ["Serving"] = NumberProperty(foodLog.Quantity),
        ["Calories"] = NumberProperty(foodLog.Calories),
        ["Proteins (g)"] = NumberProperty(foodLog.Protein),
        ["Carbonates (g)"] = NumberProperty(foodLog.Carbs),
        ["Fat (g)"] = NumberProperty(foodLog.Fat)
    };

    private static string? NullIfEmpty(string value) => string.IsNullOrEmpty(value) ? null : value;

    private static object DateProperty(DateOnly date) => new { date = new { start = date.ToString("yyyy-MM-dd") } };
    private static object SelectProperty(string value) => new { select = new { name = value } };
    private static object NumberProperty(double value) => new { number = value };

    private static object RelationProperty(string? pageId) =>
        string.IsNullOrEmpty(pageId)
            ? new { relation = Array.Empty<object>() }
            : new { relation = new[] { new { id = pageId } } };
}
