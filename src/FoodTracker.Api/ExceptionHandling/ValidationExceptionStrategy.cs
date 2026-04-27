using FoodTracker.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FoodTracker.Api.ExceptionHandling;

internal class ValidationExceptionStrategy : IExceptionHandlerStrategy
{
    public bool CanHandle(Exception exception) => exception is ValidationException;

    public async Task HandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var validationException = (ValidationException)exception;

        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Extensions = { ["errors"] = validationException.Errors }
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(problem, ct);
    }
}
