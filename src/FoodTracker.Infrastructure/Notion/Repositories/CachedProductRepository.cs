using FoodTracker.Application.Repositories;
using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Repositories;

internal class CachedProductRepository(
    IProductRepository inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options)
    : CachedRepository<Product>(inner, cache, options), IProductRepository;
