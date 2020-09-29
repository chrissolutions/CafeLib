using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.WebSockets
{
    public class WebSocketMiddleware<T> where T : IWebSocketHandler, new()
    {
        private readonly RequestDelegate _next;
        private readonly T _webSocketHandler;
        private readonly ConnectionManager _connectionManager = new ConnectionManager();

        public WebSocketMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _webSocketHandler = (T)Activator.CreateInstance(typeof(T), serviceProvider.GetService<ConnectionManager>());
        }

        public WebSocketMiddleware(RequestDelegate next, T webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var connectionId = await Connect(context);
            await Receive(connectionId);
            await _next.Invoke(context);
        }

        private async Task<Guid> Connect(HttpContext context)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionId = _connectionManager.Add(socket);
            await _webSocketHandler.OnConnect(connectionId);
            return connectionId;
        }

        private async Task Receive(Guid connectionId)
        {
            var buffer = new byte[1024 * 4];
            var socket = _connectionManager.Find(connectionId);
            if (socket == default) return;

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Binary:
                    case WebSocketMessageType.Text:
                        await _webSocketHandler.ReceiveAsync(connectionId, buffer, result.Count);
                        return;

                    case WebSocketMessageType.Close:
                        await _connectionManager.Remove(connectionId);
                        await _webSocketHandler.OnDisconnect(connectionId);
                        return;

                    default:
                        return;
                }
            }
        }
    }
}