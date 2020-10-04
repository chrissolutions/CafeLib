using System;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    public interface IWebSocketHandler : IWebSocketSender
    {
        Task OnConnect(Guid connectionId);

        Task OnDisconnect(Guid connectionId);

        Task ReceiveAsync(Guid connectionId, byte[] buffer, int count);

        Task ReceiveMessageAsync(Guid connectionId, string message);
    }
}