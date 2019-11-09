using CafeLib.Core.Extensions;

namespace CafeLib.Core.Logging
{
    public class CoreLoggerFactory : LogFactory<LogHandler>
    {
        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="messenger">log event messenger</param>
        public CoreLoggerFactory(ILogEventMessenger messenger = null)
            : base(messenger)
        {
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="category">log event category</param>
        /// <param name="messenger">log event messenger</param>
        public CoreLoggerFactory(NonNullable<string> category, ILogEventMessenger messenger)
            : base(category, messenger)
        {
        }
    }
}
