using FoodTracker.Domain.Entities;

namespace FoodTracker.Domain.Interfaces;

public interface IMacroSource
{
    ServingUnit ServingUnit { get; }
    MacroSnapshot ComputeMacros(decimal quantity);
}
