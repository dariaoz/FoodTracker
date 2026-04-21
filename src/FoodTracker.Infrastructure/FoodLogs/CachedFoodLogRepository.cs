using FoodTracker.Application.FoodLogs;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.FoodLogs;

internal class CachedFoodLogRepository(
    IFoodLogRepository inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options)
    : CachedRepository<FoodLog>(inner, cache, options), IFoodLogRepository
{
    public Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default) =>
        inner.GetByDateAsync(date, ct);
}
