using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;

namespace CafeLib.AspNet.WebSockets.Internal
{
    internal class ConnectionManager : IWebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new ConcurrentDictionary<Guid, WebSocket>();
        private readonly ConcurrentDictionary<int, Guid> _connections = new ConcurrentDictionary<int, Guid>();

        public IEnumerator<(Guid, WebSocket)> GetEnumerator()
        {
            foreach (var (key, value) in _sockets)
            {
                yield return (key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Guid Add(WebSocket socket)
        {
            var id = _connections.GetOrAdd(socket.GetHashCode(), Guid.NewGuid());
            _sockets.GetOrAdd(id, socket);
            return id;
        }

        public Guid? Find(WebSocket socket)
        {
            return _connections.TryGetValue(socket.GetHashCode(), out var connectionId) ? connectionId : default;
        }

        public WebSocket Find(Guid id)
        {
            return _sockets.TryGetValue(id, out var socket) ? socket : default;
        }

        public async Task Remove(Guid id)
        {
            if (_sockets.TryRemove(id, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Closed by {GetType().Name}", CancellationToken.None);
                socket.Dispose();
            }
        }

        public void Dispose()
        {
            Task.WaitAll(DisposeAsync().AsTask());
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(_sockets.Keys.ForEachAsync(async x => await Remove(x)));
        }
    }
}