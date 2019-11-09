using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

class TheLimitingConcurrencyPump : NotInteresting
{
    #region Hide
    const int MaxConcurrency = 3;
    CancellationTokenSource tokenSource;
    private SemaphoreSlim semaphore;
    Task pumpTask;
    #endregion

    public void Start()
    {
        tokenSource = new CancellationTokenSource();
        semaphore = new SemaphoreSlim(MaxConcurrency);

        var token = tokenSource.Token;

        pumpTask = ((Func<Task>)(async () => {
            while (!token.IsCancellationRequested) {
                
                await semaphore.WaitAsync(token).ConfigureAwait(false);

                FireAndForget(FetchAndHandleAndRelease(semaphore));
            }
        }))();
    }

    async Task FetchAndHandleAndRelease(SemaphoreSlim semaphore, CancellationToken token = default) {
        try {
            var (payload, headers) = await ReadFromQueue().ConfigureAwait(false);
            var message = Deserialize(payload, headers);

            await HandleMessage(message).ConfigureAwait(false);
        }
        finally {
            semaphore.Release();
        }
    }

    async Task HandleMessage(Message message, CancellationToken token = default) {
        await Task.Delay(1000).ConfigureAwait(false);
        Pumping(message);
    }

    public async Task Stop() {
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

        await Graceful(pumpTask).ConfigureAwait(false);

        while (semaphore.CurrentCount != MaxConcurrency) {
            await Task.Delay(50).ConfigureAwait(false);
        }

        tokenSource.Dispose();
    }
}