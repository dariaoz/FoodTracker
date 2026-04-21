using FoodTracker.Domain.Shared;

namespace FoodTracker.Domain.FoodLogs;

public class FoodLogValidator : IValidator<FoodLog>
{
    public void Validate(FoodLog entity)
    {
        List<string> errors = [];

        if (entity.Date == DateOnly.MinValue)
            errors.Add("Date is required.");

        bool hasRecipe = !string.IsNullOrWhiteSpace(entity.RecipeId);
        bool hasProduct = !string.IsNullOrWhiteSpace(entity.ProductId);

        if (!hasRecipe && !hasProduct)
            errors.Add("Either RecipeId or ProductId must be provided.");

        if (hasRecipe && hasProduct)
            errors.Add("RecipeId and ProductId cannot both be set.");

        if (entity.Quantity <= 0)
            errors.Add("Quantity must be > 0.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
