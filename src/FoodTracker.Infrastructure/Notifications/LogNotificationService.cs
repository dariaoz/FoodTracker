using FoodTracker.Application.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Infrastructure.Notifications;

internal class LogNotificationService(ILogger<LogNotificationService> logger) : INotificationService
{
    public Task NotifyAsync(string message, CancellationToken ct = default)
    {
        //todo: implement notification in telegram bot
        logger.LogWarning("Notification: {Message}", message);
        return Task.CompletedTask;
    }
}
