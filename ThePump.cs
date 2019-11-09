
using System;
using System.Threading;
using System.Threading.Tasks;

class ThePump : NotInteresting
{
    Task pumpTask;
    CancellationTokenSource tokenSource;

    public void Start() {
        tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        pumpTask = ((Func<Task>)(async () => {
            while (!token.IsCancellationRequested)
            {
                var (payload, headers) = await ReadFromQueue(token).ConfigureAwait(false);
                var message = Deserialize(payload, headers);

                await HandleMessage(message, token).ConfigureAwait(false);
            }
        }))();
    }

    async Task HandleMessage(Message message, CancellationToken token = default) {
        await Task.Delay(1000).ConfigureAwait(false);
        Pumping(message);
    }

    public async Task Stop() {
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

        await pumpTask.ConfigureAwait(false);

        tokenSource.Dispose();
    }
}