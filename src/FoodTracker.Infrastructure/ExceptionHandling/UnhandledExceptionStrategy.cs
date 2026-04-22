using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Infrastructure.ExceptionHandling;

internal class UnhandledExceptionStrategy : IExceptionHandlerStrategy
{
    private readonly ILogger<UnhandledExceptionStrategy> _logger;

    public UnhandledExceptionStrategy(ILogger<UnhandledExceptionStrategy> logger)
    {
        _logger = logger;
    }

    public bool CanHandle(Exception exception) => true;

    public async Task HandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred."
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problem, ct);
    }
}
