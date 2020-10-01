using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    public interface IWebSocketHandler : IWebSocketSender
    {
        Task OnConnect(Guid connectionId);

        Task OnDisconnect(Guid connectionId);

        Task ReceiveAsync(Guid connectionId, WebSocketMessageType messageType, byte[] buffer, int count);
    }
}