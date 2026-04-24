namespace FoodTracker.Infrastructure.Notion;

internal interface INotionClient
{
    Task<NotionDatabase> QueryDatabaseAsync(string databaseId, object? filter = null, CancellationToken ct = default);
    Task<NotionPage> GetPageAsync(string pageId, CancellationToken ct = default);
    Task<NotionPage> CreatePageAsync(string databaseId, object properties, CancellationToken ct = default);
    Task<NotionPage> UpdatePageAsync(string pageId, object properties, CancellationToken ct = default);
    Task ArchivePageAsync(string databaseId, string pageId, CancellationToken ct = default);
}
