using FoodTracker.Domain.Entities;

namespace FoodTracker.Domain.Validation;

public class ProductValidator : IValidator<Product>
{
    public void Validate(Product entity)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(entity.Name))
            errors.Add("Name is required.");

        if (entity.ServingUnit == ServingUnit.Portion)
            errors.Add("Product ServingUnit must be Gram or Milliliter.");

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
