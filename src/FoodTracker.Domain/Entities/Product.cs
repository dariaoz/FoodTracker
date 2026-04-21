using FoodTracker.Domain.Interfaces;

namespace FoodTracker.Domain.Entities;

public class Product : IHaveMacronutrients, IMacroSource, IHaveId
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ServingUnit ServingUnit { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fat { get; init; }

    public Serving Serving => new(ServingUnit, 100m);

    public MacroSnapshot ComputeMacros(decimal quantity)
    {
        double ratio = (double)quantity / 100.0;
        return new(Calories * ratio, Protein * ratio, Carbs * ratio, Fat * ratio);
    }
}
