using FoodTracker.Api.Notion.Client;

namespace FoodTracker.Api.Configuration;

public static class NotionClientExtensions
{
    public static IServiceCollection AddNotionClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotionOptions>(configuration.GetSection("Notion"));

        services.AddHttpClient<INotionClient, NotionClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.notion.com/v1/");
            client.DefaultRequestHeaders.Add("Notion-Version", "2026-03-11");
        });

        return services;
    }
}
