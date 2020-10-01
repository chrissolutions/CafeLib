using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.WebSockets
{
    public abstract class WebSocketHandler : IWebSocketHandler
    {
        protected readonly IWebSocketConnectionManager _connectionManager;
        protected readonly IWebSocketSender _sender;

        protected WebSocketHandler(IServiceProvider serviceProvider)
        {
            _connectionManager = serviceProvider.GetService<IWebSocketConnectionManager>();
            _sender = serviceProvider.GetService<IWebSocketSender>();
        }

        public virtual Task OnConnect(Guid connectionId)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnDisconnect(Guid connectionId)
        {
            return Task.CompletedTask;
        }
        public async Task BroadcastMessageAsync(string message)
        {
            await _sender.BroadcastMessageAsync(message);
        }

        public async Task SendMessageAsync(Guid socketId, string message)
        {
            await _sender.SendMessageAsync(socketId, message);
        }

        public abstract Task ReceiveAsync(Guid connectionId, WebSocketMessageType messageType, byte[] buffer, int count);
    }
}