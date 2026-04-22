namespace FoodTracker.Infrastructure.Configuration;

internal class NotionOptions
{
    public string BaseAddress { get; set; } = string.Empty;
    public string NotionVersion { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ProductsDatabaseId { get; set; } = string.Empty;
    public string RecipesDatabaseId { get; set; } = string.Empty;
    public string FoodLogDatabaseId { get; set; } = string.Empty;
}
