using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

public class TheConcurrencyPump : NotInteresting
{
    public async Task Execute()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
        var token = tokenSource.Token;

        var pumpTask = Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                FireAndForget(FetchAndHandleMessage());
            }
        });

        await pumpTask.ConfigureAwait(false);

        WriteLine("Are we done yet?");

        tokenSource.Dispose();
    }

    static async Task FetchAndHandleMessage()
    {
        var (payload, headers) = await ReadFromQueue().ConfigureAwait(false);
        var message = Deserialize(payload, headers);

        await HandleMessage(message).ConfigureAwait(false);
    }

    static async Task HandleMessage(Message message)
    {
        await Task.Delay(1000).ConfigureAwait(false);
        Pumping(message);
    }
}