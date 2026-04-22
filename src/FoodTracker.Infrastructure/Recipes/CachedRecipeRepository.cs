using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Domain.Recipes;
using FoodTracker.Infrastructure.Configuration;
using FoodTracker.Infrastructure.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FoodTracker.Infrastructure.Recipes;

internal class CachedRecipeRepository(
    IRecipeRepository inner,
    IDistributedCache cache,
    IOptions<CacheOptions> options)
    : CachedRepository<Recipe>(inner, cache, options), IRecipeRepository;
