using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Notion.Client;

namespace FoodTracker.Infrastructure.Notion.Mappers;

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
            Calories = NotionPropertyHelper.GetDouble(p, "Calories"),
            Protein = NotionPropertyHelper.GetDouble(p, "Protein"),
            Carbs = NotionPropertyHelper.GetDouble(p, "Carbs"),
            Fat = NotionPropertyHelper.GetDouble(p, "Fat")
        };
    }

    public static object ToNotionProperties(Recipe recipe) => new
    {
        Name = TitleProperty(recipe.Name),
        Servings = NumberProperty(recipe.Servings),
        Calories = NumberProperty(recipe.Calories),
        Protein = NumberProperty(recipe.Protein),
        Carbs = NumberProperty(recipe.Carbs),
        Fat = NumberProperty(recipe.Fat)
    };

    private static object TitleProperty(string value) => new { title = new[] { new { text = new { content = value } } } };
    private static object NumberProperty(double value) => new { number = value };
    private static object NumberProperty(int value) => new { number = value };
}
