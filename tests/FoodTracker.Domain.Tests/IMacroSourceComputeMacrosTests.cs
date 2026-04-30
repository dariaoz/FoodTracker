using FluentAssertions;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;

namespace FoodTracker.Domain.Tests;

public class IMacroSourceComputeMacrosTests
{
    public static TheoryData<IMacroSource> MilliliterSources() => new()
    {
        new Product { Calories = 300, Protein = 18, Fat = 11, Carbs = 14, ServingUnit = ServingUnit.Milliliter },
        new Recipe { Calories = 300, Protein = 18, Fat = 11, Carbs = 14, ServingUnit = ServingUnit.Milliliter }
    };

    public static TheoryData<IMacroSource> GramSources() => new()
    {
        new Product { Calories = 200, Protein = 10, Fat = 20, Carbs = 5,  ServingUnit = ServingUnit.Gram },
        new Recipe { Calories = 200, Protein = 10, Fat = 20, Carbs = 5, ServingUnit = ServingUnit.Gram }
    };

    public static TheoryData<IMacroSource> PortionSources() => new()
    {
        new Recipe { Calories = 200, Protein = 10, Fat = 5, Carbs = 20, ServingUnit = ServingUnit.Portion }
    };

    [Theory]
    [MemberData(nameof(MilliliterSources))]
    public void ComputeMacros_ScalesByQuantityOver100_WhenServingUnitIsMilliliter(IMacroSource source)
    {
        // Act
        var result = source.ComputeMacros(50);

        // Assert
        result.Calories.Should().Be(150);
        result.Protein.Should().Be(9);
        result.Fat.Should().Be(5.5);
        result.Carbs.Should().Be(7);
    }

    [Theory]
    [MemberData(nameof(GramSources))]
    public void ComputeMacros_ScalesByQuantityOver100_WhenServingUnitIsGram(IMacroSource source)
    {
        // Act
        var result = source.ComputeMacros(50);

        // Assert
        result.Calories.Should().Be(100);
        result.Protein.Should().Be(5);
        result.Fat.Should().Be(10);
        result.Carbs.Should().Be(2.5);
    }

    [Theory]
    [MemberData(nameof(PortionSources))]
    public void ComputeMacros_ScalesByQuantityDirectly_WhenServingUnitIsPortion(IMacroSource source)
    {
        // Act
        var result = source.ComputeMacros(3);

        // Assert
        result.Calories.Should().Be(600);
        result.Protein.Should().Be(30);
        result.Fat.Should().Be(15);
        result.Carbs.Should().Be(60);
    }
}
