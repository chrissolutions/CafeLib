using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;

namespace CafeLib.Web.SignalR
{
    internal class SignalRChannelLogFactory : LogFactory<WebChannelLogHandler>
    {
        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="category">log event category</param>
        /// <param name="messenger">log event messenger</param>
        public SignalRChannelLogFactory(NonNullable<string> category, ILogEventMessenger messenger)
            : base(category, messenger)
        {
        }
    }
}