using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion;

internal class NotionClient : INotionClient
{
    private readonly HttpClient _http;
    private readonly IDistributedCache _cache;
    private readonly ILogger<NotionClient> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly DistributedCacheEntryOptions _dataSourceIdCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };

    public NotionClient(HttpClient http, IOptions<NotionOptions> options, IDistributedCache cache, ILogger<NotionClient> logger)
    {
        _http = http;
        _cache = cache;
        _logger = logger;
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
    }

    public async Task<NotionDatabase> QueryDatabaseAsync(string databaseId, object? filter = null,
        CancellationToken ct = default)
    {
        string dataSourceId = await GetDataSourceIdAsync(databaseId, ct);

        var allResults = new List<NotionPage>();
        string? cursor = null;
        do
        {
            NotionDatabase batch = await QueryDataSourceAsync(dataSourceId, filter, cursor, ct);
            allResults.AddRange(batch.Results);
            cursor = batch.HasMore ? batch.NextCursor : null;
        } while (cursor is not null);

        _logger.LogDebug("QueryDatabaseAsync {DatabaseId} -> {DataSourceId}: {Count} results",
            databaseId, dataSourceId, allResults.Count);
        return new NotionDatabase { Results = allResults };
    }

    public async Task<NotionPage> GetPageAsync(string pageId, CancellationToken ct = default)
    {
        _logger.LogDebug("GET pages/{PageId}", pageId);
        HttpResponseMessage response = await _http.GetAsync($"pages/{pageId}", ct);
        string json = await ReadAndEnsureSuccessAsync(response, ct);
        return Deserialize<NotionPage>(json);
    }

    public async Task<NotionPage> CreatePageAsync(string databaseId, object properties, CancellationToken ct = default)
    {
        string dataSourceId = await GetDataSourceIdAsync(databaseId, ct);
        _logger.LogDebug("POST pages (data_source={DataSourceId})", dataSourceId);
        var payload = new { parent = new { data_source_id = dataSourceId, type = "data_source_id" }, properties };
        string response = await PostAsync("pages", payload, ct);
        return Deserialize<NotionPage>(response);
    }

    public async Task<NotionPage> UpdatePageAsync(string pageId, object properties, CancellationToken ct = default)
    {
        _logger.LogDebug("PATCH pages/{PageId}", pageId);
        var payload = new { properties };
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _http.PatchAsync($"pages/{pageId}", content, ct);
        string responseJson = await ReadAndEnsureSuccessAsync(response, ct);
        return Deserialize<NotionPage>(responseJson);
    }

    public async Task DeletePageAsync(string databaseId, string pageId, CancellationToken ct = default)
    {
        string dataSourceId = await GetDataSourceIdAsync(databaseId, ct);
        _logger.LogDebug("PATCH pages/{PageId} (delete, data_source={DataSourceId})", pageId, dataSourceId);
        var payload = new { in_trash = true };
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _http.PatchAsync($"pages/{pageId}", content, ct);
        await ReadAndEnsureSuccessAsync(response, ct);
    }

    private async Task<string> GetDataSourceIdAsync(string databaseId, CancellationToken ct)
    {
        string cacheKey = $"datasource:{databaseId}";
        string? cached = await _cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
        {
            _logger.LogDebug("Cache hit for datasource:{DatabaseId}", databaseId);
            return cached;
        }

        _logger.LogDebug("GET databases/{DatabaseId}", databaseId);
        HttpResponseMessage response = await _http.GetAsync($"databases/{databaseId}", ct);
        string json = await ReadAndEnsureSuccessAsync(response, ct);
        NotionDatabaseObject db = Deserialize<NotionDatabaseObject>(json);
        string dataSourceId = db.DataSources.FirstOrDefault()?.Id
            ?? throw new InvalidOperationException($"No data source found for database {databaseId}");

        await _cache.SetStringAsync(cacheKey, dataSourceId, _dataSourceIdCacheOptions, ct);
        return dataSourceId;
    }

    private async Task<NotionDatabase> QueryDataSourceAsync(string dataSourceId, object? filter, string? cursor,
        CancellationToken ct)
    {
        _logger.LogDebug("POST data_sources/{DataSourceId}/query cursor={Cursor}", dataSourceId, cursor ?? "start");
        string response = await PostAsync($"data_sources/{dataSourceId}/query", BuildQueryBody(filter, cursor), ct);
        return Deserialize<NotionDatabase>(response);
    }

    private static object BuildQueryBody(object? filter, string? cursor)
    {
        if (filter is not null && cursor is not null) return new { filter, start_cursor = cursor, result_type = "page", in_trash = false };
        if (filter is not null) return new { filter, result_type = "page", in_trash = false };
        if (cursor is not null) return new { start_cursor = cursor, result_type = "page", in_trash = false };
        return new { result_type = "page", in_trash = false };
    }

    private async Task<string> PostAsync(string path, object payload, CancellationToken ct)
    {
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _http.PostAsync(path, content, ct);
        return await ReadAndEnsureSuccessAsync(response, ct);
    }

    private async Task<string> ReadAndEnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        string body = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Notion API error: {StatusCode} {Reason} for {Method} {Path} — {Body}",
                (int)response.StatusCode, response.ReasonPhrase, response.RequestMessage?.Method,
                response.RequestMessage?.RequestUri?.PathAndQuery, body);
            throw new HttpRequestException(
                $"Notion API error: {(int)response.StatusCode} {response.ReasonPhrase} — {body}");
        }
        return body;
    }

    private static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _jsonOptions)
        ?? throw new InvalidOperationException("Notion API returned null response.");
}
