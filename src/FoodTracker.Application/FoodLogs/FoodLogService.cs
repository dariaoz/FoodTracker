using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FoodTracker.Application.FoodLogs;

public class FoodLogService : IFoodLogService
{
    private readonly INotionContext _context;
    private readonly IValidator<FoodLog> _validator;
    private readonly ILogger<FoodLogService> _logger;

    public FoodLogService(INotionContext context, IValidator<FoodLog> validator, ILogger<FoodLogService> logger)
    {
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public Task<IList<FoodLog>> GetAllAsync(CancellationToken ct = default) => _context.FoodLogs.GetAllAsync(ct);
    public Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default) => _context.FoodLogs.GetByDateAsync(date, ct);
    public Task<FoodLog?> GetByIdAsync(string id, CancellationToken ct = default) => _context.FoodLogs.GetByIdAsync(id, ct);

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
        _logger.LogInformation("Created food log {Id} for {SourceType} {SourceId} on {Date}", created.Id, sourceType, sourceId, foodLog.Date);
        return created;
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _context.FoodLogs.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted food log {Id}", id);
    }

    private async Task<(IMacroSource Source, string SourceType, string SourceId)> ResolveSourceAsync(FoodLog foodLog, CancellationToken ct)
    {
        var isProduct = foodLog.ProductId is not null;
        var sourceType = isProduct ? "product" : "recipe";
        var sourceId = isProduct ? foodLog.ProductId! : foodLog.RecipeId!;

        IMacroSource source = isProduct
            ? await _context.Products.GetByIdAsync(sourceId, ct) ?? throw new ValidationException([$"Product '{sourceId}' not found."])
            : await _context.Recipes.GetByIdAsync(sourceId, ct) ?? throw new ValidationException([$"Recipe '{sourceId}' not found."]);

        return (source, sourceType, sourceId);
    }
}
