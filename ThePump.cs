
using System;
using System.Threading;
using System.Threading.Tasks;

public class ThePump : NotInteresting
{
    public static async Task Execute()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        var token = tokenSource.Token;

        var pumpTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var (payload, headers) = await ReadFromQueue();
                var message = Deserialize(payload, headers);

                await HandleMessage(message).ConfigureAwait(false);
            }
        });

        await pumpTask.ConfigureAwait(false);

        tokenSource.Dispose();
    }

    static Task HandleMessage(Message message)
    {
        Pumping(message);
        return Task.Delay(1000);
    }
}