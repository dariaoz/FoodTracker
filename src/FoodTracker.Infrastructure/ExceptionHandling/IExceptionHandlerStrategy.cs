using Microsoft.AspNetCore.Http;

namespace FoodTracker.Infrastructure.ExceptionHandling;

internal interface IExceptionHandlerStrategy
{
    bool CanHandle(Exception exception);
    Task HandleAsync(HttpContext context, Exception exception, CancellationToken ct);
}
