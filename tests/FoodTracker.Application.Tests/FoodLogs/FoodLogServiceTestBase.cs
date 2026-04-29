using FoodTracker.Application.FoodLogs;
using FoodTracker.Application.FoodLogs.Interfaces;
using FoodTracker.Application.Shared;
using FoodTracker.Domain.FoodLogs;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FoodTracker.Application.Tests.FoodLogs;

public abstract class FoodLogServiceTestBase
{
    protected readonly FoodLogService Sut;
    protected readonly INotionContext Context = Substitute.For<INotionContext>();
    protected readonly ISearchContext SearchContext = Substitute.For<ISearchContext>();
    protected readonly IIndexingService Indexing = Substitute.For<IIndexingService>();
    protected readonly ILogger<FoodLogService> Logger = Substitute.For<ILogger<FoodLogService>>();
    protected readonly IFoodLogSearchRepository FoodLogSearchRepo = Substitute.For<IFoodLogSearchRepository>();

    protected FoodLogServiceTestBase()
    {
        SearchContext.FoodLogs.Returns(FoodLogSearchRepo);
        Sut = new FoodLogService(Context, SearchContext, new FoodLogValidator(), Indexing, Logger);
    }

    protected void SetupSyncToInvokeCallback() =>
        Indexing.SyncIndexAsync(Arg.Any<Func<Task>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(async ci => await ci.Arg<Func<Task>>()());
}
