namespace FoodTracker.Domain.Shared;

public interface IHaveMacronutrients : IHaveCalories
{
    double Protein { get; }
    double Carbs { get; }
    double Fat { get; }
}
