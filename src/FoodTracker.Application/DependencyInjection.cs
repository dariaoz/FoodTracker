using FoodTracker.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAppValidators();
        services.AddAppServices();
    }
}
