using FoodTracker.Api.Notion.Client;

namespace FoodTracker.Api.Notion.Mappers;

internal static class NotionPropertyHelper
{
    public static string GetString(Dictionary<string, NotionPropertyValue> props, string key)
    {
        if (!props.TryGetValue(key, out NotionPropertyValue? prop))
        {
            return string.Empty;
        }

        return prop.Type switch
        {
            "title" => prop.Title?.FirstOrDefault()?.PlainText ?? string.Empty,
            "rich_text" => prop.RichText?.FirstOrDefault()?.PlainText ?? string.Empty,
            _ => string.Empty
        };
    }

    public static double GetDouble(Dictionary<string, NotionPropertyValue> props, string key) =>
        props.TryGetValue(key, out NotionPropertyValue? prop) ? prop.Number ?? 0d : 0d;

    public static decimal GetDecimal(Dictionary<string, NotionPropertyValue> props, string key) =>
        props.TryGetValue(key, out NotionPropertyValue? prop) ? (decimal)(prop.Number ?? 0d) : 0m;

    public static string GetSelect(Dictionary<string, NotionPropertyValue> props, string key) =>
        props.TryGetValue(key, out NotionPropertyValue? prop) ? prop.Select?.Name ?? string.Empty : string.Empty;

    public static DateOnly GetDate(Dictionary<string, NotionPropertyValue> props, string key)
    {
        if (!props.TryGetValue(key, out NotionPropertyValue? prop) || prop.Date is null)
        {
            return DateOnly.MinValue;
        }

        return DateOnly.Parse(prop.Date.Start);
    }
}
