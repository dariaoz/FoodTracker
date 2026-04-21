namespace FoodTracker.Domain.Shared;

public interface IMacroSource
{
    ServingUnit ServingUnit { get; }
    MacroSnapshot ComputeMacros(decimal quantity);
}
