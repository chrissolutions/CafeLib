using System;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    public abstract class WebSocketHandler : IWebSocketHandler
    {
        private readonly IWebSocketSender _webSocketSender;

        protected WebSocketHandler(IWebSocketSender sender)
        {
            _webSocketSender = sender;
        }

        public abstract Task OnConnect(Guid connectionId);

        public abstract Task OnDisconnect(Guid connectionId);

        public abstract Task<bool> ReceiveAsync(Guid connectionId, byte[] buffer, int count);

        public async Task BroadcastMessageAsync(string message)
        {
            await _webSocketSender.BroadcastMessageAsync(message);
        }

        public async Task SendMessageAsync(Guid connectionId, string message)
        {
            await _webSocketSender.SendMessageAsync(connectionId, message);
        }
    }
}