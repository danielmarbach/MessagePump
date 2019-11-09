using System;
using System.Threading;
using System.Threading.Tasks;

public class ThePumpHowItProbablyLooksLikeInReality : NotInteresting
{
    private const int MaxConcurrency = 3;
    private CancellationTokenSource tokenSource;
    private SemaphoreSlim semaphore;
    private Task pumpTask;

    public void Start() {
        tokenSource = new CancellationTokenSource();

        var token = tokenSource.Token;

        semaphore = new SemaphoreSlim(MaxConcurrency);

        pumpTask = ((Func<Task>)(async () => {
            while (!token.IsCancellationRequested)
            {
                await semaphore.WaitAsync(token);

                FireAndForget(FetchAndHandleAndReleaseWithMiddleware(semaphore, token));
            }
        }))();
    }

    public async Task Stop() {
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        await Graceful(pumpTask).ConfigureAwait(false);

        while (semaphore.CurrentCount != MaxConcurrency) {
            await Task.Delay(50).ConfigureAwait(false);
        }

        tokenSource.Dispose();
    }

    async Task FetchAndHandleAndReleaseWithMiddleware(SemaphoreSlim semaphore, CancellationToken token = default) {
        using (var transaction = CreateTransaction()) {
            var (payload, headers) = await ReadFromQueue(token, transaction).ConfigureAwait(false);
            var message = Deserialize(payload, headers);

            using (var childServiceProvider = CreateChildServiceProvider()) {
                try {
                    var middlewareFuncs = new Func<HandlerContext, Func<HandlerContext, CancellationToken, Task>, CancellationToken, Task>[] { Middleware1, Middleware2 };
                    var middleware = FlextensibleMiddleware(childServiceProvider, middlewareFuncs, token);

                    await middleware(message).ConfigureAwait(false);

                    transaction.Complete();
                }
                catch (Exception) {
                    // Just log?
                }
                finally {
                    semaphore.Release();
                }
            }
        }
    }

    async Task Middleware1(HandlerContext context, Func<HandlerContext, CancellationToken, Task> next, CancellationToken token = default) {
        await next(context, token);
    }

    async Task Middleware2(HandlerContext context, Func<HandlerContext, CancellationToken, Task> next, CancellationToken token = default) {

        var handlers = context.Provider.Resolve<IHandleMessage<Message>>();

        foreach (var handler in handlers) {
            await handler.Handle(context.Message, context, token).ConfigureAwait(false);
        }

        await next(context, token).ConfigureAwait(false);
    }

    class MessageHandler : IHandleMessage<Message>
    {
        public async Task Handle(Message message, HandlerContext context, CancellationToken token = default)
        {
            await Task.Delay(1000).ConfigureAwait(false);
            Pumping(message);
            await context.Send(new Message()).ConfigureAwait(false);
        }
    }
}