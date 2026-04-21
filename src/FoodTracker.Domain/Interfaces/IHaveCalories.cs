using FoodTracker.Domain.Entities;

namespace FoodTracker.Domain.Interfaces;

public interface IHaveCalories
{
    double Calories { get; }
    Serving Serving { get; }
}
