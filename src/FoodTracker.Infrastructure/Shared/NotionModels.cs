using System.Text.Json.Serialization;

namespace FoodTracker.Infrastructure.Shared;

internal class NotionDatabase
{
    [JsonPropertyName("results")]
    public List<NotionPage> Results { get; set; } = [];

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}

internal class NotionPage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public Dictionary<string, NotionPropertyValue> Properties { get; set; } = [];
}

internal class NotionPropertyValue
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public List<NotionRichText>? Title { get; set; }

    [JsonPropertyName("rich_text")]
    public List<NotionRichText>? RichText { get; set; }

    [JsonPropertyName("number")]
    public double? Number { get; set; }

    [JsonPropertyName("select")]
    public NotionSelectOption? Select { get; set; }

    [JsonPropertyName("date")]
    public NotionDate? Date { get; set; }
}

internal class NotionRichText
{
    [JsonPropertyName("plain_text")]
    public string PlainText { get; set; } = string.Empty;
}

internal class NotionSelectOption
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

internal class NotionDate
{
    [JsonPropertyName("start")]
    public string Start { get; set; } = string.Empty;
}
