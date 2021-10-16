using System;
using CafeLib.Core.Logging;
using Microsoft.AspNetCore.Connections;

namespace CafeLib.Web.SignalR.ConnectionFactory
{
    internal class SignalRTextConnectionFactory : SignalRConnectionFactory
    {
        public SignalRTextConnectionFactory(Action<LogEventMessage> listener)
            : base(TransferFormat.Text, listener)
        {
        }
    }
}
