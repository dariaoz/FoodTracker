namespace FoodTracker.Api.Domain.Validation;

public interface IValidator<in T>
{
    void Validate(T entity);
}