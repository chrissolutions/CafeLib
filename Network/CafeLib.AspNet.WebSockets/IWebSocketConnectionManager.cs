using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    public interface IWebSocketConnectionManager : IEnumerable<(Guid, WebSocket)>
    {
        Guid Add(WebSocket socket);
        Guid? Find(WebSocket socket);
        WebSocket Find(Guid id);
        Task Remove(Guid id);
    }
}