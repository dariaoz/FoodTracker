using FoodTracker.Infrastructure.Notion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Configuration;

internal static class NotionClientExtensions
{
    public static void AddNotionClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotionOptions>(configuration.GetSection("Notion"));

        services.AddHttpClient<INotionClient, NotionClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<NotionOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseAddress);
            client.DefaultRequestHeaders.Add("Notion-Version", options.NotionVersion);
        });
    }
}
