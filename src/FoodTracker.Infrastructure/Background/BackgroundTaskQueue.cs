using System.Threading.Channels;
using FoodTracker.Application.Shared;

namespace FoodTracker.Infrastructure.Background;

internal class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue =
        Channel.CreateUnbounded<Func<CancellationToken, Task>>();

    public void Enqueue(Func<CancellationToken, Task> workItem) =>
        _queue.Writer.TryWrite(workItem);

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken ct) =>
        await _queue.Reader.ReadAsync(ct);
}
