
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
public class NotInteresting
{
    protected NotInteresting() 
    {
        Message.ResetId();
    }

    protected static void Pumping(Message message) => WriteLine($"Pumping {message.Id}...");

    protected static Transaction CreateTransaction()
    {
        return new Transaction();
    }

    protected static Task<(Memory<byte> payload, Memory<byte> headers)> ReadFromQueue(Transaction transaction = null) => Task.FromResult((new Memory<byte>(), new Memory<byte>()));

    protected static Message Deserialize(Memory<byte> payload, Memory<byte> headers) => new Message();

    protected static ChildServiceProvider CreateChildServiceProvider()
    {
        return new ChildServiceProvider();
    }

    protected static Func<Message, Task> FlextensibleMiddleware(ChildServiceProvider provider, params Func<HandlerContext, Func<HandlerContext, Task>, Task>[] middleware)
    {
        return (msg) =>
        {
            var context = new HandlerContext() { Message = msg };
            return middleware[1](context, (msg1) => middleware[0](msg1, msg2 => Done(msg2)));
        };
    }

    static Task Done(HandlerContext msg)
    {
        return Task.FromResult(0);
    }

    protected static void FireAndForget(Task task)
    {
    }

    protected static async Task Graceful(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
    }

    protected class Message
    {
        static long idForDemo;

        public long Id { get; }

        public Message() => Id = Interlocked.Increment(ref idForDemo);

        public static void ResetId()
        {
            idForDemo = 0;
        }
    }

    protected class ChildServiceProvider : IDisposable
    {
        public IEnumerable<T> Resolve<T>()
        {
            return Enumerable.Empty<T>();
        }

        public void Dispose()
        {

        }
    }

    protected interface IHandleMessage<TMessage> where TMessage : class
    {
        Task Handle(TMessage message, HandlerContext context);
    }

    protected class HandlerContext
    {
        public Message Message { get; set; }

        public ChildServiceProvider Provider { get; set; }
        public Task Send(object message)
        {
            return Task.FromResult(0);
        }
    }

    protected class Transaction : IDisposable
    {
        public void Dispose()
        {
        }

        public void Complete()
        {
        }
    }
}
