using CafeLib.Core.Extensions;

namespace CafeLib.Core.Logging
{
    internal class CoreLoggerProvider : LogProvider<LogHandler>
    {
        #region Constructors

        /// <summary>
        /// CoreLoggerProvider default constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="messenger">log event messenger</param>
        public CoreLoggerProvider(NonNullable<string> category, ILogEventMessenger messenger)
            : base(category, messenger)
        {
        }

        #endregion
    }
}
