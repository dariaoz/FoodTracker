using FluentAssertions;
using FoodTracker.Application.Products.Interfaces;
using FoodTracker.Application.Recipes.Interfaces;
using FoodTracker.Domain.FoodLogs;
using FoodTracker.Domain.Products;
using FoodTracker.Domain.Recipes;
using FoodTracker.Domain.Shared;
using NSubstitute;

namespace FoodTracker.Application.Tests.FoodLogs;

public class FoodLogServiceCreateAsyncTests : FoodLogServiceTestBase
{
    private readonly IRecipeSearchRepository _recipeSearchRepo = Substitute.For<IRecipeSearchRepository>();
    private readonly IProductSearchRepository _productSearchRepo = Substitute.For<IProductSearchRepository>();

    private const double Tolerance = 0.1;

    public FoodLogServiceCreateAsyncTests()
    {
        SearchContext.Recipes.Returns(_recipeSearchRepo);
        SearchContext.Products.Returns(_productSearchRepo);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenDateIsNotSet()
    {
        // Arrange
        var foodLog = new FoodLog
        {
            Quantity = 1,
            RecipeId = "recipe-q",
            ServingUnit = ServingUnit.Portion
        };
        var message = "Date is required.";

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenProductIdAndRecipeIdIsNotProvided()
    {
        // Arrange
        var foodLog = new FoodLog
        {
            Date = new DateOnly(2026, 1, 1),
            Quantity = 1,
            ServingUnit = ServingUnit.Portion
        };
        var message = "Either RecipeId or ProductId must be provided.";

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenProductIdAndRecipeIdSetBoth()
    {
        // Arrange
        var foodLog = new FoodLog
        {
            Date = new DateOnly(2026, 1, 1),
            Quantity = 1,
            ProductId = "product-q",
            RecipeId = "recipe-q",
            ServingUnit = ServingUnit.Portion
        };
        var message = "RecipeId and ProductId cannot both be set.";

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task CreateAsync_ShouldThrowValidationException_WhenQuantityIsZeroOrNegative(double quantity)
    {
        // Arrange
        var foodLog = new FoodLog
        {
            Date = new DateOnly(2026, 1, 1),
            Quantity = quantity,
            RecipeId = "recipe-q",
            ServingUnit = ServingUnit.Portion
        };
        var message = "Quantity must be > 0.";

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenProductIdIsNotFound()
    {
        // Arrange
        var foodLog = new FoodLog
        {
            Quantity = 1,
            Date = new DateOnly(2026, 1, 1),
            ProductId = "product-q",
            ServingUnit = ServingUnit.Gram
        };
        var message = $"Product '{foodLog.ProductId}' not found.";
        _productSearchRepo.GetByIdAsync(foodLog.ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenRecipeIdIsNotFound()
    {
        // Arrange
        var foodLog = new FoodLog
        {
            Quantity = 1,
            Date = new DateOnly(2026, 1, 1),
            RecipeId = "recipe-q",
            ServingUnit = ServingUnit.Gram
        };
        var message = $"Recipe '{foodLog.RecipeId}' not found.";
        _recipeSearchRepo.GetByIdAsync(foodLog.RecipeId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenServingUnitDoesntMatchProductServingUnit()
    {
        // Arrange
        var source = new Product
        {
            Id = "product-q",
            ServingUnit = ServingUnit.Gram
        };
        var foodLog = new FoodLog
        {
            Quantity = 1,
            Date = new DateOnly(2026, 1, 1),
            ProductId = source.Id,
            ServingUnit = ServingUnit.Milliliter
        };
        var message = $"ServingUnit must match the source's ServingUnit ({source.ServingUnit}).";
        _productSearchRepo.GetByIdAsync(foodLog.ProductId, Arg.Any<CancellationToken>())
            .Returns(source);

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenServingUnitDoesntMatchRecipeServingUnit()
    {
        // Arrange
        var source = new Recipe()
        {
            Id = "recipe-q",
            ServingUnit = ServingUnit.Portion
        };
        var foodLog = new FoodLog
        {
            Quantity = 1,
            Date = new DateOnly(2026, 1, 1),
            RecipeId = source.Id,
            ServingUnit = ServingUnit.Gram
        };
        var message = $"ServingUnit must match the source's ServingUnit ({source.ServingUnit}).";
        _recipeSearchRepo.GetByIdAsync(foodLog.RecipeId, Arg.Any<CancellationToken>())
            .Returns(source);

        // Act
        var requestAction = async () => await Sut.CreateAsync(foodLog);

        // Assert
        await requestAction.Should()
            .ThrowAsync<ValidationException>().WithMessage($"Validation failed: {message}");
    }

    private (FoodLog input, FoodLog expected) SetupProductScenario()
    {
        var source = new Product
        {
            Id = "product-q",
            Calories = 150.0,
            Protein = 9.0,
            Carbs = 5.0,
            Fat = 3.0,
            ServingUnit = ServingUnit.Gram
        };
        var input = new FoodLog
        {
            Quantity = 300,
            Date = new DateOnly(2026, 1, 1),
            ProductId = source.Id,
            ServingUnit = source.ServingUnit
        };
        var expected = new FoodLog
        {
            Id = "id-q",
            Calories = source.Calories * input.Quantity / 100,
            Protein = source.Protein * input.Quantity / 100,
            Fat = source.Fat * input.Quantity / 100,
            Carbs = source.Carbs * input.Quantity / 100,
            Quantity = input.Quantity,
            Date = input.Date,
            ProductId = input.ProductId,
            ServingUnit = input.ServingUnit
        };
        _productSearchRepo.GetByIdAsync(source.Id, Arg.Any<CancellationToken>()).Returns(source);
        Context.FoodLogs.CreateAsync(Arg.Any<FoodLog>(), Arg.Any<CancellationToken>()).Returns(expected);
        SetupSyncToInvokeCallback();
        return (input, expected);
    }

    [Fact]
    public async Task CreateAsync_ShouldPassComputedMacrosToNotion_WhenProductSourceInGram()
    {
        // Arrange
        var (input, expected) = SetupProductScenario();

        // Act
        await Sut.CreateAsync(input);

        // Assert
        await Context.FoodLogs.Received(1)
            .CreateAsync(Arg.Is<FoodLog>(f =>
                Math.Abs(f.Calories - expected.Calories) < Tolerance &&
                Math.Abs(f.Protein - expected.Protein) < Tolerance &&
                Math.Abs(f.Fat - expected.Fat) < Tolerance &&
                Math.Abs(f.Carbs - expected.Carbs) < Tolerance), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldSyncIndexAfterCreate_WhenProductSource()
    {
        // Arrange
        var (input, expected) = SetupProductScenario();

        // Act
        await Sut.CreateAsync(input);

        // Assert
        await SearchContext.FoodLogs.Received(1).IndexAsync(expected, Arg.Any<CancellationToken>());
        await Indexing.Received(1).SyncIndexAsync(Arg.Any<Func<Task>>(),
            Arg.Is($"index food log {expected.Id}"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFoodLogFromNotion_WhenProductSource()
    {
        // Arrange
        var (input, expected) = SetupProductScenario();

        // Act
        var result = await Sut.CreateAsync(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    private (FoodLog input, FoodLog expected) SetupRecipeScenario()
    {
        var source = new Recipe
        {
            Id = "recipe-q",
            Calories = 200.0,
            Protein = 8.0,
            Carbs = 15.0,
            Fat = 9.0,
            ServingUnit = ServingUnit.Portion
        };
        var input = new FoodLog
        {
            Quantity = 1.5,
            Date = new DateOnly(2026, 1, 1),
            RecipeId = source.Id,
            ServingUnit = source.ServingUnit
        };
        var expected = new FoodLog
        {
            Id = "id-q",
            Calories = source.Calories * input.Quantity,
            Protein = source.Protein * input.Quantity,
            Fat = source.Fat * input.Quantity,
            Carbs = source.Carbs * input.Quantity,
            Quantity = input.Quantity,
            Date = input.Date,
            RecipeId = input.RecipeId,
            ServingUnit = input.ServingUnit
        };
        _recipeSearchRepo.GetByIdAsync(source.Id, Arg.Any<CancellationToken>()).Returns(source);
        Context.FoodLogs.CreateAsync(Arg.Any<FoodLog>(), Arg.Any<CancellationToken>()).Returns(expected);
        SetupSyncToInvokeCallback();
        return (input, expected);
    }

    [Fact]
    public async Task CreateAsync_ShouldPassComputedMacrosToNotion_WhenRecipeSourceInPortion()
    {
        // Arrange
        var (input, expected) = SetupRecipeScenario();

        // Act
        await Sut.CreateAsync(input);

        // Assert
        await Context.FoodLogs.Received(1)
            .CreateAsync(Arg.Is<FoodLog>(f =>
                Math.Abs(f.Calories - expected.Calories) < Tolerance &&
                Math.Abs(f.Protein - expected.Protein) < Tolerance &&
                Math.Abs(f.Fat - expected.Fat) < Tolerance &&
                Math.Abs(f.Carbs - expected.Carbs) < Tolerance), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldSyncIndexAfterCreate_WhenRecipeSource()
    {
        // Arrange
        var (input, expected) = SetupRecipeScenario();

        // Act
        await Sut.CreateAsync(input);

        // Assert
        await SearchContext.FoodLogs.Received(1).IndexAsync(expected, Arg.Any<CancellationToken>());
        await Indexing.Received(1).SyncIndexAsync(Arg.Any<Func<Task>>(),
            Arg.Is($"index food log {expected.Id}"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFoodLogFromNotion_WhenRecipeSource()
    {
        // Arrange
        var (input, expected) = SetupRecipeScenario();

        // Act
        var result = await Sut.CreateAsync(input);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
