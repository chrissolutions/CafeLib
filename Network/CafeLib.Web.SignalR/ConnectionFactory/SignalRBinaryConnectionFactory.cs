using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace CafeLib.Web.SignalR
{
    public class WebChannelConnectionFactory : IConnectionFactory
    {
        private readonly Uri _url;
        private readonly ILoggerFactory _loggerFactory;
        private HttpConnection _connection;

        public WebChannelConnectionFactory(Uri url, ILoggerFactory loggerFactory = null)
        {
            _url = url;
            _loggerFactory = loggerFactory;
        }

        public async Task<ConnectionContext> ConnectAsync(TransferFormat transferFormat, CancellationToken cancellationToken = new CancellationToken())
        {
            _connection = new HttpConnection(_url, HttpTransportType.WebSockets, _loggerFactory);
            await _connection.StartAsync(transferFormat, cancellationToken);
            return _connection;
        }

        public async Task DisposeAsync(ConnectionContext connection)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
