using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.WebSockets
{
    public abstract class WebSocketHandler : IWebSocketHandler
    {
        private readonly IWebSocketConnectionManager _connectionManager;
        private readonly IWebSocketSender _sender;

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

        public virtual Task ReceiveAsync(Guid connectionId, byte[] buffer, int count)
        {
            return Task.CompletedTask;
        }

        public virtual Task ReceiveMessageAsync(Guid connectionId, string message)
        {
            return Task.CompletedTask;
        }
    }
}