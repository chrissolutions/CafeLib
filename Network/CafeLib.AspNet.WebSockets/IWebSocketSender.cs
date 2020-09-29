using System;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    public interface IWebSocketSender
    {
        Task BroadcastMessageAsync(string message);

        Task SendMessageAsync(Guid connectionId, string message);
    }
}