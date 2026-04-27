using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNotionClient(configuration);
        services.AddRedisCache(configuration);
        services.AddNotionRepositories();
    }
}
