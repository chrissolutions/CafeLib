using CafeLib.Core.MethodBinding;
using Microsoft.Extensions.Logging;

namespace CafeLib.Web.SignalR
{
    public abstract class SignalRTextChannel : SignalRChannel
    {
        protected SignalRTextChannel(string url, MethodBridge bridge, ILogger logger = null)
            : base(url, bridge, logger)
        {
        }
    }
}
