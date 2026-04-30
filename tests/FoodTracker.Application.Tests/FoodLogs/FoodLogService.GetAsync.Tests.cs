using FluentAssertions;
using FoodTracker.Application.FoodLogs;
using FoodTracker.Domain.FoodLogs;
using NSubstitute;

namespace FoodTracker.Application.Tests.FoodLogs;

public class FoodLogServiceGetAsyncTests : FoodLogServiceTestBase
{
    [Fact]
    public async Task GetAsync_ShouldReturnEmptyList_WhenNoFoodLogsExist()
    {
        // Arrange
        var filter = new FoodLogFilter();
        FoodLogSearchRepo.SearchAsync(filter, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await Sut.GetAsync(filter);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnList_WhenSomeFoodLogsExist()
    {
        // Arrange
        var expectedResult = new List<FoodLog> { new FoodLog { Id = "1" }, new FoodLog { Id = "2" } };
        var filter = new FoodLogFilter();
        FoodLogSearchRepo.SearchAsync(filter, Arg.Any<CancellationToken>())
            .Returns(expectedResult.AsReadOnly());

        // Act
        var result = await Sut.GetAsync(filter);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetAsync_ShouldForwardFilterToRepository_WhenFilterHasValues()
    {
        // Arrange
        var filter = new FoodLogFilter(new DateOnly(2026, 1, 1), new DateOnly(2026, 2, 1));
        FoodLogSearchRepo.SearchAsync(filter, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        await Sut.GetAsync(filter);

        // Assert
        await FoodLogSearchRepo.Received(1).SearchAsync(filter, Arg.Any<CancellationToken>());
    }
}
