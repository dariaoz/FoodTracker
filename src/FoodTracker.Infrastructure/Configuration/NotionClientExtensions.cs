using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure.Configuration;

internal static class NotionClientExtensions
{
    public static void AddNotionClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotionOptions>(configuration.GetSection("Notion"));

        services.AddHttpClient<INotionClient, NotionClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.notion.com/v1/");
            client.DefaultRequestHeaders.Add("Notion-Version", "2026-03-11");
        });
    }
}
