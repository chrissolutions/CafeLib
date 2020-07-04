using System;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    public class LogProvider<T> : LoggerProvider where T : ILogger
    {
        #region Constructors

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="listener"></param>
        public LogProvider(Action<LogEventMessage> listener)
            : this(new LogEventReceiver(listener))
        {
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="receiver">log event receiver</param>
        public LogProvider(ILogEventReceiver receiver)
            : base(receiver)
        {
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">log event receiver</param>
        internal LogProvider(string category, ILogEventReceiver receiver)
            : base(category, receiver)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the logger.
        /// </summary>
        /// <param name="category">log category</param>
        /// <returns>logger</returns>
        public override ILogger CreateLogger(string category)
        {
            Category ??= category;
            return (T)Activator.CreateInstance(typeof(T), Category, Receiver);
        }

        #endregion
    }
}
