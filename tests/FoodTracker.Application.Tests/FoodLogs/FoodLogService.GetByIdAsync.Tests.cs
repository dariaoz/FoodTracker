using FluentAssertions;
using FoodTracker.Domain.FoodLogs;
using NSubstitute;

namespace FoodTracker.Application.Tests.FoodLogs;

public class FoodLogServiceGetByIdAsyncTests : FoodLogServiceTestBase
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenFoodLogWitThatIdDoesntExist()
    {
        // Arrange
        FoodLogSearchRepo.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((FoodLog?)null);

        // Act
        var result = await Sut.GetByIdAsync("1");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFoodLog_WhenFoodLogByIdExist()
    {
        // Arrange
        var expectedResult = new FoodLog { Id = "1" };
        FoodLogSearchRepo.GetByIdAsync(expectedResult.Id, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await Sut.GetByIdAsync(expectedResult.Id);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
