using System;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CafeLib.Core.Logging
{
    public class LogHandler : ILogger
    {
        #region Properties

        protected string Category { get; }
        protected ILogEventMessenger Messenger { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// LoggerHandler constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="messenger">log event messenger</param>
        public LogHandler(string category, ILogEventMessenger messenger)
        {
            Category = category;
            Messenger = messenger ?? new LogNullEventMessenger();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Log message.
        /// </summary>
        /// <typeparam name="TState">typeof state</typeparam>
        /// <param name="logLevel">log level</param>
        /// <param name="eventId">event id</param>
        /// <param name="state">state</param>
        /// <param name="exception">exception</param>
        /// <param name="formatter">message formatter</param>
        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter?.Invoke(state, exception);
            Messenger.LogMessage(new LogEventMessage(Category, this.ToErrorLevel(logLevel), new LogEventInfo(eventId), message, exception));
        }

        /// <summary>
        /// Queries whether the logger is enabled.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Obtains the logging scope.
        /// </summary>
        /// <typeparam name="TState">state type</typeparam>
        /// <param name="state">state</param>
        /// <returns>scope object</returns>
        public virtual IDisposable BeginScope<TState>(TState state)
        {
            return NullLogger.Instance.BeginScope(state);
        }

        #endregion
    }
}
