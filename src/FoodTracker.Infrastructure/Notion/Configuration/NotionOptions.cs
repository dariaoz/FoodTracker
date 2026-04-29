namespace FoodTracker.Infrastructure.Notion.Configuration;

internal class NotionOptions
{
    public string BaseAddress { get; init; } = null!;
    public string NotionVersion { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
    public string ProductsDatabaseId { get; init; } = null!;
    public string RecipesDatabaseId { get; init; } = null!;
    public string FoodLogDatabaseId { get; init; } = null!;
}
