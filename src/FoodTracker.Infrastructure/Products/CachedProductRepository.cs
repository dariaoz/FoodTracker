using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Domain.Products;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Products;

internal class CachedProductRepository(
    IProductRepository inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options)
    : CachedRepository<Product>(inner, cache, options), IProductRepository;
