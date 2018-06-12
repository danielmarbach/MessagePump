using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

public class TheConcurrencyPump : NotInteresting
{
    public static async Task Execute()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        var token = tokenSource.Token;

        var pumpTask = Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                var (payload, headers) = ReadFromQueue();
                var message = Deserialize(payload, headers);

                FireAndForget(HandleMessage(message));
            }
        });

        await pumpTask.ConfigureAwait(false);

        WriteLine("Are we done yet?");

        tokenSource.Dispose();
    }

    static async Task HandleMessage(Message message)
    {
        await Task.Delay(1000);
        Pumping(message);
    }
}