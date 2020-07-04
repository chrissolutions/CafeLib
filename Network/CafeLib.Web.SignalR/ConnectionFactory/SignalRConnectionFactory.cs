using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.Extensions.Logging;

namespace CafeLib.Web.SignalR.ConnectionFactory
{
    public abstract class SignalRConnectionFactory : IConnectionFactory
    {
        private readonly TransferFormat _transferFormat;
        private readonly ILoggerFactory _loggerFactory;
        private HttpConnection _connection;

        protected SignalRConnectionFactory(TransferFormat transferFormat, ILoggerFactory loggerFactory = null)
        {
            _transferFormat = transferFormat;
            _loggerFactory = loggerFactory;
        }

        public async ValueTask<ConnectionContext> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = new CancellationToken())
        {
            _connection = new HttpConnection(((UriEndPoint)endpoint).Uri, HttpTransportType.WebSockets, _loggerFactory) ;
            await _connection.StartAsync(_transferFormat, cancellationToken);
            return _connection;
        }
    }
}
