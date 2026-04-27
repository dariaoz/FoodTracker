namespace FoodTracker.Domain.Shared;

public interface IMacroSource : IHaveMacronutrients
{
    ServingUnit ServingUnit { get; }
    double ServingQuantity => ServingUnit == ServingUnit.Portion ? 1.0 : 100.0;

    MacroSnapshot ComputeMacros(double quantity)
    {
        var ratio = quantity / ServingQuantity;
        return new(Calories * ratio, Protein * ratio, Carbs * ratio, Fat * ratio);
    }
}
