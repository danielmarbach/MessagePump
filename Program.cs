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
            await ThePump.Execute();
            WriteLine("Press to continue...");
            ReadLine();
            Clear();

            Console.WriteLine($"{nameof(TheConcurrencyPump)} executing");
            await TheConcurrencyPump.Execute();
            WriteLine("Press to continue...");
            ReadLine();
            Clear();

            Console.WriteLine($"{nameof(TheLimitingConcurrencyPump)} executing");
            await TheLimitingConcurrencyPump.Execute();
        }
    }
}
