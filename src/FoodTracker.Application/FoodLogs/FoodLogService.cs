using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Shared;

namespace FoodTracker.Application.FoodLogs;

public class FoodLogService : IFoodLogService
{
    private readonly INotionContext _context;
    private readonly IValidator<FoodLog> _validator;

    public FoodLogService(INotionContext context, IValidator<FoodLog> validator)
    {
        _context = context;
        _validator = validator;
    }

    public Task<IList<FoodLog>> GetAllAsync(CancellationToken ct = default) => _context.FoodLogs.GetAllAsync(ct);
    public Task<IList<FoodLog>> GetByDateAsync(DateOnly date, CancellationToken ct = default) => _context.FoodLogs.GetByDateAsync(date, ct);
    public Task<FoodLog?> GetByIdAsync(string id, CancellationToken ct = default) => _context.FoodLogs.GetByIdAsync(id, ct);

    public async Task<FoodLog> CreateAsync(FoodLog foodLog, CancellationToken ct = default)
    {
        _validator.Validate(foodLog);

        IMacroSource source = await ResolveSourceAsync(foodLog, ct);

        if (foodLog.ServingUnit != source.ServingUnit)
            throw new ValidationException([$"ServingUnit must match the source's ServingUnit ({source.ServingUnit})."]);

        MacroSnapshot macros = source.ComputeMacros(foodLog.Quantity);

        return await _context.FoodLogs.CreateAsync(new FoodLog
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
    }

    public Task DeleteAsync(string id, CancellationToken ct = default) => _context.FoodLogs.DeleteAsync(id, ct);

    private async Task<IMacroSource> ResolveSourceAsync(FoodLog foodLog, CancellationToken ct) =>
        foodLog.ProductId is not null
            ? await _context.Products.GetByIdAsync(foodLog.ProductId, ct)
                ?? throw new ValidationException([$"Product '{foodLog.ProductId}' not found."])
            : await _context.Recipes.GetByIdAsync(foodLog.RecipeId!, ct)
                ?? throw new ValidationException([$"Recipe '{foodLog.RecipeId}' not found."]);
}
