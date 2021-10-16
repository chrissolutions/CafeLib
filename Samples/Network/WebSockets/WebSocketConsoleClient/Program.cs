using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketConsoleClient
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await RunWebSockets();
        }

        private static async Task RunWebSockets()
        {
            var client = new ClientWebSocket();

            await client.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);

            Console.WriteLine("Connected!");

            var sending = Task.Run(async () =>
            {
                string line;
                while ((line = Console.ReadLine()) != null && line != string.Empty)
                {
                    var bytes = Encoding.UTF8.GetBytes(line);
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            });

            var receiving = Receiving(client);

            await Task.WhenAll(sending, receiving);
        }

        private static async Task Receiving(WebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                else if (client.State != WebSocketState.Closed && result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
    }
}
