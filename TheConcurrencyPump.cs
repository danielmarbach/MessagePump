using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

class TheConcurrencyPump : NotInteresting
{
    Task pumpTask;
    CancellationTokenSource tokenSource;

    public void Start()
    {
        tokenSource = new CancellationTokenSource();
        
        var token = tokenSource.Token;

        pumpTask = Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                FireAndForget(FetchAndHandleMessage(token));
            }
        });
    }

    public async Task Stop() 
    {
        tokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));

        await pumpTask.ConfigureAwait(false);

        WriteLine("Are we done yet?");

        tokenSource.Dispose();
    }

    async Task FetchAndHandleMessage(CancellationToken token = default)
    {
        var (payload, headers) = await ReadFromQueue().ConfigureAwait(false);
        var message = Deserialize(payload, headers);

        await HandleMessage(message).ConfigureAwait(false);
    }

    async Task HandleMessage(Message message, CancellationToken token = default)
    {
        await Task.Delay(1000).ConfigureAwait(false);
        Pumping(message);
    }
}