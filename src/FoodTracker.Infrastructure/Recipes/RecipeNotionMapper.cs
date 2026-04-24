using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;
using FoodTracker.Infrastructure.Notion;

namespace FoodTracker.Infrastructure.Recipes;

internal static class RecipeNotionMapper
{
    public static Recipe ToEntity(NotionPage page)
    {
        Dictionary<string, NotionPropertyValue> p = page.Properties;
        return new Recipe
        {
            Id = page.Id,
            Name = NotionPropertyHelper.GetString(p, "Name"),
            Servings = (int)NotionPropertyHelper.GetDouble(p, "Servings"),
            ServingUnit = NotionPropertyHelper.GetEnum<ServingUnit>(p, "Serving Unit"),
            Calories = NotionPropertyHelper.GetDouble(p, "Calories"),
            Protein = NotionPropertyHelper.GetDouble(p, "Protein (g)"),
            Carbs = NotionPropertyHelper.GetDouble(p, "Carbs (g)"),
            Fat = NotionPropertyHelper.GetDouble(p, "Fat (g)")
        };
    }

    public static Dictionary<string, object> ToNotionProperties(Recipe recipe) => new()
    {
        ["Name"] = TitleProperty(recipe.Name),
        ["Servings"] = NumberProperty(recipe.Servings),
        ["Serving Unit"] = SelectProperty(recipe.ServingUnit.ToString()),
        ["Calories"] = NumberProperty(recipe.Calories),
        ["Protein (g)"] = NumberProperty(recipe.Protein),
        ["Carbs (g)"] = NumberProperty(recipe.Carbs),
        ["Fat (g)"] = NumberProperty(recipe.Fat)
    };

    private static object TitleProperty(string value) => new { title = new[] { new { text = new { content = value } } } };
    private static object SelectProperty(string value) => new { select = new { name = value } };
    private static object NumberProperty(double value) => new { number = value };
    private static object NumberProperty(int value) => new { number = value };
}
