using FoodTracker.Application.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Infrastructure.Background;

internal class StartupReindexService(
    IBackgroundTaskQueue queue,
    IIndexingService indexingService,
    IHostEnvironment env,
    ILogger<StartupReindexService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken ct)
    {
        if (!env.IsDevelopment())
        {
            logger.LogInformation("Enqueueing startup reindex");
            queue.Enqueue(ctq => indexingService.ReindexAsync(ctq));
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
