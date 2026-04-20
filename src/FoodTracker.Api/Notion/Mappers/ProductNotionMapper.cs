using FoodTracker.Api.Domain.Entities;
using FoodTracker.Api.Notion.Client;

namespace FoodTracker.Api.Notion.Mappers;

internal static class ProductNotionMapper
{
    public static Product ToEntity(NotionPage page)
    {
        Dictionary<string, NotionPropertyValue> p = page.Properties;
        return new Product
        {
            Id = page.Id,
            Name = NotionPropertyHelper.GetString(p, "Name"),
            ServingUnit = Enum.Parse<ServingUnit>(NotionPropertyHelper.GetSelect(p, "ServingUnit"), ignoreCase: true),
            Calories = NotionPropertyHelper.GetDouble(p, "Calories"),
            Protein = NotionPropertyHelper.GetDouble(p, "Protein"),
            Carbs = NotionPropertyHelper.GetDouble(p, "Carbs"),
            Fat = NotionPropertyHelper.GetDouble(p, "Fat")
        };
    }

    public static object ToNotionProperties(Product product) => new
    {
        Name = TitleProperty(product.Name),
        ServingUnit = SelectProperty(product.ServingUnit.ToString()),
        Calories = NumberProperty(product.Calories),
        Protein = NumberProperty(product.Protein),
        Carbs = NumberProperty(product.Carbs),
        Fat = NumberProperty(product.Fat)
    };

    private static object TitleProperty(string value) => new { title = new[] { new { text = new { content = value } } } };
    private static object SelectProperty(string value) => new { select = new { name = value } };
    private static object NumberProperty(double value) => new { number = value };
}
