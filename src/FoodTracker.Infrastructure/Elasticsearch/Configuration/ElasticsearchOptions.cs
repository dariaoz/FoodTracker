namespace FoodTracker.Infrastructure.Elasticsearch.Configuration;

internal class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";
    public required string Url { get; init; }
    public required string FoodLogsIndex { get; init; }
    public required string ProductsIndex { get; init; }
    public required string RecipesIndex { get; init; }
}
