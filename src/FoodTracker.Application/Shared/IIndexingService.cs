namespace FoodTracker.Application.Shared;

public interface IIndexingService
{
    Task SyncAsync(Func<Task> esAction, string operation, CancellationToken ct);
    Task ReindexAsync(CancellationToken ct);
}
