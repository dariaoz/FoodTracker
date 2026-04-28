namespace FoodTracker.Application.Shared;

public interface INotificationService
{
    Task NotifyAsync(string message, CancellationToken ct = default);
}
