﻿using System;
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
        /// <param name="logEvent"></param>
        public LoggerProvider(Action<LogEventMessage> logEvent)
            : this(new LogEventReceiver(logEvent))
        {
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="receiver">log event receiver</param>
        public LoggerProvider(ILogEventReceiver receiver)
            : this(null, receiver)
        {
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">log event receiver</param>
        internal LoggerProvider(string category, ILogEventReceiver receiver)
        {
            Category = category ?? string.Empty;
            Receiver = receiver ?? new LogEventReceiver();
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
            return new LoggerCore(category, Receiver);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose() { }

        #endregion
    }
}