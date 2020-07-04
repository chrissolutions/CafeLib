using CafeLib.Core.Extensions;

namespace CafeLib.Core.Logging
{
    internal class CoreLoggerProvider : LogProvider<LogEventSender>
    {
        #region Constructors

        /// <summary>
        /// CoreLoggerProvider default constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">log event message receiver</param>
        public CoreLoggerProvider(NonNullable<string> category, NonNullable<ILogEventReceiver> receiver)
            : base(category, receiver)
        {
        }

        #endregion
    }
}
