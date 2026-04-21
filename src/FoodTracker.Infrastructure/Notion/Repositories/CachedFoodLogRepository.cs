using FoodTracker.Application.Repositories;
using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Repositories;

internal class CachedFoodLogRepository(
    IFoodLogRepository inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options)
    : CachedRepository<FoodLog>(inner, cache, options), IFoodLogRepository
{
    public Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default) =>
        inner.GetByDateAsync(date, ct);
}
