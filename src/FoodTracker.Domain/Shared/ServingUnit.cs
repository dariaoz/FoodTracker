using System.Text.Json.Serialization;

namespace FoodTracker.Domain.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServingUnit
{
    Gram,
    Milliliter,
    Portion
}
