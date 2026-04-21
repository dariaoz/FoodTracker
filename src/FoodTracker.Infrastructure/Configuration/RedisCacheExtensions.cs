using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTracker.Infrastructure.Configuration;

internal static class RedisCacheExtensions
{
    public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheOptions>(configuration.GetSection("Cache"));
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration["Redis:ConnectionString"]);
    }
}
