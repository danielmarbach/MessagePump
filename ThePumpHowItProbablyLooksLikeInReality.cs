using System;
using System.Threading;
using System.Threading.Tasks;

public class ThePumpHowItProbablyLooksLikeInReality : NotInteresting
{
    private const int MaxConcurrency = 3;

    public static async Task Execute()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        var token = tokenSource.Token;

        var semaphore = new SemaphoreSlim(MaxConcurrency);

        var pumpTask = ((Func<Task>)(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await semaphore.WaitAsync(token);

                FireAndForget(FetchAndHandleAndReleaseWithMiddleware(semaphore));
            }
        }))();

        await Graceful(pumpTask).ConfigureAwait(false);

        while (semaphore.CurrentCount != MaxConcurrency)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }

        tokenSource.Dispose();
    }

    static async Task FetchAndHandleAndReleaseWithMiddleware(SemaphoreSlim semaphore)
    {
        using (var transaction = CreateTransaction()) 
        {
            var (payload, headers) = await ReadFromQueue(transaction).ConfigureAwait(false);
            var message = Deserialize(payload, headers);

            using (var childServiceProvider = CreateChildServiceProvider())
            {
                try
                {
                    var middlewareFuncs = new Func<HandlerContext, Func<HandlerContext, Task>, Task>[] { Middleware1, Middleware2 };
                    var middleware = FlextensibleMiddleware(childServiceProvider, middlewareFuncs);
                    
                    await middleware(message).ConfigureAwait(false);

                    transaction.Complete();
                }
                catch (Exception)
                {
                    // Just log?
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }
    }

    static async Task Middleware1(HandlerContext context, Func<HandlerContext, Task> next) {
        
        await next(context);
    }

    static async Task Middleware2(HandlerContext context, Func<HandlerContext, Task> next) {
        
        var handlers = context.Provider.Resolve<IHandleMessage<Message>>();

        foreach (var handler in handlers)
        {
            await handler.Handle(context.Message, context).ConfigureAwait(false);
        }
        
        await next(context).ConfigureAwait(false);
    }

    class MessageHandler : IHandleMessage<Message>
    {
        public async Task Handle(Message message, HandlerContext context)
        {
            await Task.Delay(1000).ConfigureAwait(false);
            Pumping(message);
            await context.Send(new Message()).ConfigureAwait(false);
        }
    }
}