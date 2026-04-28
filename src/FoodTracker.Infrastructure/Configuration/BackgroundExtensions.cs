using FoodTracker.Application.Shared;
using FoodTracker.Infrastructure.Background;
using FoodTracker.Infrastructure.Elasticsearch;
using FoodTracker.Infrastructure.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure.Configuration;

internal static class BackgroundExtensions
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddSingleton<INotificationService, LogNotificationService>();
        services.AddSingleton<IIndexingService, IndexingService>();
        services.AddHostedService<ReindexBackgroundService>();
        services.AddHostedService<StartupReindexService>();
    }
}
