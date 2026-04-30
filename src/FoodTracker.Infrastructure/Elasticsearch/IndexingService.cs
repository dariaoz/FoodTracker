using FoodTracker.Application.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Infrastructure.Elasticsearch;

internal class IndexingService : IIndexingService
{
    private readonly IEnumerable<IReindexable> _reindexables;
    private readonly IBackgroundTaskQueue _queue;
    private readonly INotificationService _notificationService;
    private readonly ILogger<IndexingService> _logger;

    public IndexingService(
        IEnumerable<IReindexable> reindexables,
        IBackgroundTaskQueue queue,
        INotificationService notificationService,
        ILogger<IndexingService> logger)
    {
        _reindexables = reindexables;
        _queue = queue;
        _notificationService = notificationService;
        _logger = logger;
    }
    public async Task SyncIndexAsync(Func<Task> esAction, string operation, CancellationToken ct)
    {
        try
        {
            await esAction();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ES sync failed for {Operation}, enqueueing reindex", operation);
            await _notificationService.NotifyAsync($"ES sync failed for {operation}, reindex queued", ct);
            _queue.Enqueue(async ctq =>
            {
                try
                {
                    await ReindexAsync(ctq);
                    await _notificationService.NotifyAsync($"Reindex succeeded after ES failure ({operation})", ctq);
                }
                catch (Exception reindexEx)
                {
                    await _notificationService.NotifyAsync(
                        $"Critical: reindex failed after ES failure ({operation}): {reindexEx.Message}", ctq);
                }
            });
        }
    }

    public async Task ReindexAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting full reindex");
        foreach (var reindexable in _reindexables)
            await reindexable.ReindexAsync(ct);
        _logger.LogInformation("Reindex complete");
    }
}
