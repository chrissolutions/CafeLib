using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CafeLib.AspNet.WebSockets
{
    public interface IConnectionManager : IEnumerable<(Guid, WebSocket)>, IAsyncDisposable, IDisposable
    {
        Guid Add(WebSocket socket);
        Guid? Find(WebSocket socket);
        WebSocket Find(Guid id);
        Task Remove(Guid id);
    }
}