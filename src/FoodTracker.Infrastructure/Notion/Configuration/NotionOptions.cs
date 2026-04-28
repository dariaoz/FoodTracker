namespace FoodTracker.Infrastructure.Notion.Configuration;

internal class NotionOptions
{
    public string BaseAddress { get; set; }
    public string NotionVersion { get; set; }
    public string ApiKey { get; set; }
    public string ProductsDatabaseId { get; set; }
    public string RecipesDatabaseId { get; set; }
    public string FoodLogDatabaseId { get; set; }
}
