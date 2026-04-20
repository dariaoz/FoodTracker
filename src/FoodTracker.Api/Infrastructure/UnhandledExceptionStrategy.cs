using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.Infrastructure;

public class UnhandledExceptionStrategy : IExceptionHandlerStrategy
{
    public bool CanHandle(Exception exception) => true;

    public async Task HandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred."
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problem, ct);
    }
}