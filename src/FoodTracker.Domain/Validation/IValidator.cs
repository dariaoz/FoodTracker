namespace FoodTracker.Domain.Validation;

public interface IValidator<in T>
{
    void Validate(T entity);
}
