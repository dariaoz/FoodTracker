namespace FoodTracker.Api.Infrastructure;

public interface IExceptionHandlerStrategy
{
    bool CanHandle(Exception exception);
    Task HandleAsync(HttpContext context, Exception exception, CancellationToken ct);
}
