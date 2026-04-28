namespace FoodTracker.Application.Shared;

public interface IReindexable
{
    Task ReindexAsync(CancellationToken ct);
}
