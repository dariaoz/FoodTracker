using FoodTracker.Api.ExceptionHandling;

namespace FoodTracker.Api.Configuration;

internal static class ExceptionExtensions
{
    public static void AddExceptionHandling(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandlerStrategy, ValidationExceptionStrategy>();
        services.AddSingleton<IExceptionHandlerStrategy, UnhandledExceptionStrategy>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
