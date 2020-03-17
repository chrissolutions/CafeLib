using CafeLib.Core.Extensions;

namespace CafeLib.Core.Logging
{
    public class CoreLoggerFactory : LogFactory<LogEventSender>
    {
        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="receiver">log event receiver</param>
        public CoreLoggerFactory(ILogEventReceiver receiver = null)
            : base(receiver)
        {
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="category">log event category</param>
        /// <param name="receiver">log event receiver</param>
        public CoreLoggerFactory(NonNullable<string> category, ILogEventReceiver receiver)
            : base(category, receiver)
        {
        }
    }
}
