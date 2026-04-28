using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Application.FoodLogs;

public class FoodLogService : IFoodLogService
{
    private readonly INotionContext _context;
    private readonly ISearchContext _searchContext;
    private readonly IValidator<FoodLog> _validator;
    private readonly IIndexingService _indexing;
    private readonly ILogger<FoodLogService> _logger;

    public FoodLogService(
        INotionContext context,
        ISearchContext searchContext,
        IValidator<FoodLog> validator,
        IIndexingService indexing,
        ILogger<FoodLogService> logger)
    {
        _context = context;
        _searchContext = searchContext;
        _validator = validator;
        _indexing = indexing;
        _logger = logger;
    }

    public Task<IList<FoodLog>> GetAsync(FoodLogFilter filter, CancellationToken ct = default) =>
        _searchContext.FoodLogs.SearchAsync(filter, ct);

    public Task<FoodLog?> GetByIdAsync(string id, CancellationToken ct = default) =>
        _searchContext.FoodLogs.GetByIdAsync(id, ct);

    public async Task<FoodLog> CreateAsync(FoodLog foodLog, CancellationToken ct = default)
    {
        _validator.Validate(foodLog);

        var (source, sourceType, sourceId) = await ResolveSourceAsync(foodLog, ct);

        if (foodLog.ServingUnit != source.ServingUnit)
            throw new ValidationException([$"ServingUnit must match the source's ServingUnit ({source.ServingUnit})."]);

        var macros = source.ComputeMacros(foodLog.Quantity);

        var created = await _context.FoodLogs.CreateAsync(new()
        {
            Id = foodLog.Id,
            Date = foodLog.Date,
            ProductId = foodLog.ProductId,
            RecipeId = foodLog.RecipeId,
            ServingUnit = foodLog.ServingUnit,
            Quantity = foodLog.Quantity,
            Calories = macros.Calories,
            Protein = macros.Protein,
            Carbs = macros.Carbs,
            Fat = macros.Fat
        }, ct);

        _logger.LogInformation("Created food log {Id} for {SourceType} {SourceId} on {Date}",
            created.Id, sourceType, sourceId, foodLog.Date);

        await _indexing.SyncAsync(() => _searchContext.FoodLogs.IndexAsync(created, ct), $"index food log {created.Id}", ct);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _context.FoodLogs.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted food log {Id}", id);
        await _indexing.SyncAsync(() => _searchContext.FoodLogs.DeleteAsync(id, ct), $"delete food log {id}", ct);
    }

    private async Task<(IMacroSource Source, string SourceType, string SourceId)> ResolveSourceAsync(
        FoodLog foodLog, CancellationToken ct)
    {
        var isProduct = foodLog.ProductId is not null;
        var sourceType = isProduct ? "product" : "recipe";
        var sourceId = isProduct ? foodLog.ProductId! : foodLog.RecipeId!;

        IMacroSource source = isProduct
            ? await _searchContext.Products.GetByIdAsync(sourceId, ct)
              ?? throw new ValidationException([$"Product '{sourceId}' not found."])
            : await _searchContext.Recipes.GetByIdAsync(sourceId, ct)
              ?? throw new ValidationException([$"Recipe '{sourceId}' not found."]);

        return (source, sourceType, sourceId);
    }
}
