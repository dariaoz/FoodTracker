using NSubstitute;

namespace FoodTracker.Application.Tests.FoodLogs;

public class FoodLogServiceDeleteAsyncTests : FoodLogServiceTestBase
{
    [Fact]
    public async Task DeleteAsync_ShouldReceiveNotionDeleteAndSyncIndexWithCorrectId_WhenInvoked()
    {
        // Arrange
        var id = "id-1";
        SetupSyncToInvokeCallback();

        // Act
        await Sut.DeleteAsync(id);

        // Assert
        await Context.FoodLogs.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
        await FoodLogSearchRepo.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
        await Indexing.Received(1).SyncIndexAsync(Arg.Any<Func<Task>>(),
            Arg.Is($"delete food log {id}"),
            Arg.Any<CancellationToken>());

    }
}
