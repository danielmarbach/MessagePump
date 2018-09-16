using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

public class TheLimitingConcurrencyPump : NotInteresting
{
    private const int MaxConcurrency = 3;

    public async Task Execute()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        var token = tokenSource.Token;

        var semaphore = new SemaphoreSlim(MaxConcurrency);

        var pumpTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await semaphore.WaitAsync(token);

                FireAndForget(FetchAndHandleAndRelease(semaphore));
            }
        });

        await Graceful(pumpTask).ConfigureAwait(false);

        while (semaphore.CurrentCount != MaxConcurrency)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }

        tokenSource.Dispose();
    }

    static async Task FetchAndHandleAndRelease(SemaphoreSlim semaphore)
    {
        try
        {
            var (payload, headers) = await ReadFromQueue();
            var message = Deserialize(payload, headers);

            await HandleMessage(message).ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    static async Task HandleMessage(Message message)
    {
        await Task.Delay(1000);
        Pumping(message);
    }
}