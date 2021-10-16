using System;
using System.Threading.Tasks;
using CafeLib.Web.SignalR;

namespace SignalRChannelTest
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var channel = new SignalRChannel("https://localhost:44350/test", new ClientBridge(), new TestLogWriter());
            channel.Changed += x =>
            {
                Console.WriteLine($"Changed: {x.ConnectionState}");
            };

            channel.Advised += x =>
            {
                Console.WriteLine($"Advised: {x.Message}");
            };

            await channel.Start();

            await channel.InvokeAsync("notifymessage", "Hello World");
            await channel.InvokeAsync("doclientstuff", "doclientstuff", "Calling doClientStuff");
            await channel.InvokeAsync("doguidtest", "Calling doGuidTest");

            Console.ReadLine();
        }
    }
}
