
using System;
using System.Threading;
using System.Threading.Tasks;

public class ThePump : NotInteresting
{
    public async Task Execute()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        var token = tokenSource.Token;

        var pumpTask = ((Func<Task>)(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var (payload, headers) = await ReadFromQueue().ConfigureAwait(false);
                var message = Deserialize(payload, headers);

                await HandleMessage(message).ConfigureAwait(false);
            }
        }))();

        await pumpTask.ConfigureAwait(false);

        tokenSource.Dispose();
    }

    static async Task HandleMessage(Message message)
    {
        await Task.Delay(1000).ConfigureAwait(false);
        Pumping(message);
    }
}