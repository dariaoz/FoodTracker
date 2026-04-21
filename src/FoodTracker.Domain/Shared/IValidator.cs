namespace FoodTracker.Domain.Shared;

public interface IValidator<in T>
{
    void Validate(T entity);
}
