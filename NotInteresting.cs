
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
public class NotInteresting
{
    protected static void Pumping(Message message) => WriteLine($"Pumping {message.Id}...");

    protected static (Memory<byte> payload, Memory<byte> headers) ReadFromQueue() => (new Memory<byte>(), new Memory<byte>());

    protected static Message Deserialize(Memory<byte> payload, Memory<byte> headers) => new Message();

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
    }
}
