using FoodTracker.Api.Domain.Entities;

namespace FoodTracker.Api.Domain.Validation;

public class RecipeValidator : IValidator<Recipe>
{
    public void Validate(Recipe entity)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(entity.Name))
            errors.Add("Name is required.");

        if (entity.Servings <= 0)
            errors.Add("Servings must be > 0.");

        if (entity.Calories < 0)
            errors.Add("Calories must be >= 0.");

        if (entity.Protein < 0)
            errors.Add("Protein must be >= 0.");

        if (entity.Carbs < 0)
            errors.Add("Carbs must be >= 0.");

        if (entity.Fat < 0)
            errors.Add("Fat must be >= 0.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
