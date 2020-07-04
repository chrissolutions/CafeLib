using System;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    public class LoggerProvider : ILoggerProvider
    {
        #region Automatic Properties

        public string Category { get; private protected set; }
        private protected ILogEventReceiver Receiver { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="listener"></param>
        public LoggerProvider(Action<LogEventMessage> listener)
            : this(new LogEventReceiver(listener))
        {
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="receiver">log event receiver</param>
        public LoggerProvider(NonNullable<ILogEventReceiver> receiver)
        {
            Receiver = receiver.Value ?? new LogEventReceiver(null);
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">log event receiver</param>
        internal LoggerProvider(NonNullable<string> category, NonNullable<ILogEventReceiver> receiver)
        {
            Category = category.Value;
            Receiver = receiver.Value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the logger.
        /// </summary>
        /// <param name="category">log category</param>
        /// <returns>logger</returns>
        public virtual ILogger CreateLogger(string category)
        {
            Category ??= category;
            return new LogEventSender(category, Receiver);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose() { }

        #endregion
    }
}