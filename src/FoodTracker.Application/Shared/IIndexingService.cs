namespace FoodTracker.Application.Shared;

public interface IIndexingService
{
    Task SyncIndexAsync(Func<Task> esAction, string operation, CancellationToken ct);
    Task ReindexAsync(CancellationToken ct);
}
