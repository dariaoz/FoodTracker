using Microsoft.AspNetCore.Diagnostics;

namespace FoodTracker.Api.ExceptionHandling;

internal class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IEnumerable<IExceptionHandlerStrategy> _strategies;

    public GlobalExceptionHandler(IEnumerable<IExceptionHandlerStrategy> strategies)
    {
        _strategies = strategies;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(exception));
        if (strategy is null)
        {
            return false;
        }

        await strategy.HandleAsync(context, exception, ct);
        return true;
    }
}
