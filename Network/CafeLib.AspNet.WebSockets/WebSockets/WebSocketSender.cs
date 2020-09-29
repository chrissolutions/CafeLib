using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    internal class WebSocketSender : IWebSocketSender
    {
        private readonly IConnectionManager _connectionManager;

        protected WebSocketSender(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;            
        }

        public async Task BroadcastMessageAsync(string message)
        {
            foreach (var (k, v) in _connectionManager)
            {
                if (v.State == WebSocketState.Open)
                    await SendMessageAsync(k, message);
            }
        }

        public async Task SendMessageAsync(Guid connectionId, string message)
        {
            var socket = _connectionManager.Find(connectionId);
            if (socket == default || socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(
                new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}