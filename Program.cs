using System;
using System.Threading.Tasks;
using static System.Console;

namespace MessagePump
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // await ThePump.Execute();

            // ReadLine();

            // await TheConcurrencyPump.Execute();

            ReadLine();

            await TheLimitingConcurrencyPump.Execute();
        }
    }
}
