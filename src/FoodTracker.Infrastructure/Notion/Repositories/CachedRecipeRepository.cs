using FoodTracker.Application.Repositories;
using FoodTracker.Domain.Entities;
using FoodTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Notion.Repositories;

internal class CachedRecipeRepository(
    IRecipeRepository inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options)
    : CachedRepository<Recipe>(inner, cache, options), IRecipeRepository;
