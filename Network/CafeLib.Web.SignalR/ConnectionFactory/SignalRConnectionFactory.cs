using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Logging;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.Extensions.Logging;

namespace CafeLib.Web.SignalR.ConnectionFactory
{
    internal class SignalRConnectionFactory : IConnectionFactory
    {
        private readonly TransferFormat _transferFormat;
        private readonly Action<LogEventMessage> _listener;
        private HttpConnection _connection;

        public SignalRConnectionFactory(Action<LogEventMessage> listener)
            : this (TransferFormat.Binary, listener)
        {
        }

        public SignalRConnectionFactory(TransferFormat transferFormat, Action<LogEventMessage> listener)
        {
            _transferFormat = transferFormat;
            _listener = listener;
        }

        public async ValueTask<ConnectionContext> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var factory = LoggerFactory.Create(builder => builder.AddProvider(new LoggerProvider(_listener)));
            _connection = new HttpConnection(((UriEndPoint)endpoint).Uri, HttpTransportType.WebSockets, factory) ;
            await _connection.StartAsync(_transferFormat, cancellationToken);
            return _connection;
        }
    }
}
