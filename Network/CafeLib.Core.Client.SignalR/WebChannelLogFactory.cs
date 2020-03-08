using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;

namespace CafeLib.Core.Client.SignalR
{
    internal class WebChannelLogFactory : LogFactory<WebChannelLogHandler>
    {
        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="category">log event category</param>
        /// <param name="messenger">log event messenger</param>
        public WebChannelLogFactory(NonNullable<string> category, ILogEventMessenger messenger)
            : base(category, messenger)
        {
        }
    }
}