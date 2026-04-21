using FoodTracker.Infrastructure.ExceptionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure.Configuration;

internal static class ExceptionExtensions
{
    public static void AddExceptionHandling(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandlerStrategy, ValidationExceptionStrategy>();
        services.AddSingleton<IExceptionHandlerStrategy, UnhandledExceptionStrategy>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
