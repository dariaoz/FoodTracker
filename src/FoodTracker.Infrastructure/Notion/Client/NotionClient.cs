using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Client;

internal class NotionClient : INotionClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public NotionClient(HttpClient http, IOptions<NotionOptions> options)
    {
        _http = http;
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
    }

    public async Task<NotionDatabase> QueryDatabaseAsync(string databaseId, object? filter = null,
        CancellationToken ct = default)
    {
        var allResults = new List<NotionPage>();
        string? cursor = null;

        do
        {
            string response = await PostAsync($"databases/{databaseId}/query", BuildQueryBody(filter, cursor), ct);
            NotionDatabase page = Deserialize<NotionDatabase>(response);
            allResults.AddRange(page.Results);
            cursor = page.HasMore ? page.NextCursor : null;
        } while (cursor is not null);

        return new NotionDatabase { Results = allResults };
    }

    private static object BuildQueryBody(object? filter, string? cursor)
    {
        if (filter is not null && cursor is not null) return new { filter, start_cursor = cursor };
        if (filter is not null) return new { filter };
        if (cursor is not null) return new { start_cursor = cursor };
        return new { };
    }

    public async Task<NotionPage> GetPageAsync(string pageId, CancellationToken ct = default)
    {
        HttpResponseMessage response = await _http.GetAsync($"pages/{pageId}", ct);
        EnsureSuccess(response);
        string json = await response.Content.ReadAsStringAsync(ct);
        return Deserialize<NotionPage>(json);
    }

    public async Task<NotionPage> CreatePageAsync(string databaseId, object properties, CancellationToken ct = default)
    {
        var payload = new { parent = new { database_id = databaseId }, properties };
        string response = await PostAsync("pages", payload, ct);
        return Deserialize<NotionPage>(response);
    }

    public async Task<NotionPage> UpdatePageAsync(string pageId, object properties, CancellationToken ct = default)
    {
        var payload = new { properties };
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _http.PatchAsync($"pages/{pageId}", content, ct);
        EnsureSuccess(response);
        string responseJson = await response.Content.ReadAsStringAsync(ct);
        return Deserialize<NotionPage>(responseJson);
    }

    public async Task ArchivePageAsync(string pageId, CancellationToken ct = default)
    {
        var payload = new { in_trash = true };
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _http.PatchAsync($"pages/{pageId}", content, ct);
        EnsureSuccess(response);
    }

    private async Task<string> PostAsync(string path, object payload, CancellationToken ct)
    {
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _http.PostAsync(path, content, ct);
        EnsureSuccess(response);
        return await response.Content.ReadAsStringAsync(ct);
    }

    private static void EnsureSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Notion API error: {(int)response.StatusCode} {response.ReasonPhrase}");
    }

    private static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, JsonOptions)
        ?? throw new InvalidOperationException("Notion API returned null response.");
}
