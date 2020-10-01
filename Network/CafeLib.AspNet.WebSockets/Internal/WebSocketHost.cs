﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CafeLib.AspNet.WebSockets.Internal
{
    internal class WebSocketHost<T> where T : IWebSocketHandler
    {
        private readonly T _webSocketHandler;
        private readonly IWebSocketConnectionManager _connectionManager;

        public WebSocketHost(RequestDelegate next, IServiceProvider serviceProvider)
        {
            var _ = next;
            _webSocketHandler = serviceProvider.GetService<T>();
            _connectionManager = serviceProvider.GetService<IWebSocketConnectionManager>();
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var connectionId = await Connect(context);
            await Receive(connectionId);
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

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                await ProcessMessage(connectionId, result, buffer);
            }
        }

        private async Task ProcessMessage(Guid connectionId, WebSocketReceiveResult result, byte[] buffer)
        {
            switch (result.MessageType)
            {
                case WebSocketMessageType.Binary:
                case WebSocketMessageType.Text:
                    await _webSocketHandler.ReceiveAsync(connectionId, result.MessageType, buffer, result.Count);
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