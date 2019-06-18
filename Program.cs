using System;
using System.Threading.Tasks;
using static System.Console;

namespace MessagePump
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"{nameof(ThePump)} executing");
            var pump = new ThePump();
            pump.Start();
            await pump.Stop();
            WriteLine("Press to continue...");
            ReadLine();
            Clear();

            Console.WriteLine($"{nameof(TheConcurrencyPump)} executing");
            var concurrencyPump = new TheConcurrencyPump();
            concurrencyPump.Start();
            await concurrencyPump.Stop();
            WriteLine("Press to continue...");
            ReadLine();
            Clear();

            Console.WriteLine($"{nameof(TheLimitingConcurrencyPump)} executing");
            var limittingPump = new TheLimitingConcurrencyPump();
            limittingPump.Start();
            await limittingPump.Stop();
        }
    }
}
