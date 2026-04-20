using FoodTracker.Api.Domain.Entities;

namespace FoodTracker.Api.Domain.Interfaces;

public interface IHaveCalories
{
    double Calories { get; }
    Serving Serving { get; }
}
