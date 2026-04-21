namespace FoodTracker.Infrastructure.Configuration;

internal class CacheOptions
{
    public int TtlMinutes { get; set; } = 15;
    public TimeSpan Ttl => TimeSpan.FromMinutes(TtlMinutes);
}
