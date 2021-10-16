using System;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    /// <summary>
    /// Log event writer.
    /// </summary>
    public class LogEventWriter : LoggerBase, ILogEventWriter
    {
        #region Priavate Variables

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// LogEventWriter constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="logEventListener">log event listener</param>
        public LogEventWriter(NonNullable<string> category, Action<LogEventMessage> logEventListener)
            : this(category, new LogEventReceiver(logEventListener))
        {
        }

        /// <summary>
        /// LoggerHandler constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">logger receiver</param>
        public LogEventWriter(string category, ILogEventReceiver receiver)
            : base(category, receiver)
        {
            var factory = LoggerFactory.Create(builder => builder.AddProvider(new LoggerProvider(Receiver)));
            _logger = factory.CreateLogger(category);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Info(string message) => _logger.Info(message);

        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Info(LogEventInfo eventInfo, string message) => _logger.Info(eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Error(string message) => _logger.Error(LogEventInfo.Empty, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Error(string message, Exception exception) => _logger.Error(LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Error(LogEventInfo eventInfo, string message) => _logger.Error(eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Error(LogEventInfo eventInfo, string message, Exception exception) => _logger.Error(eventInfo, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Critical(string message) => _logger.Critical(LogEventInfo.Empty, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Critical(LogEventInfo eventInfo, string message) => _logger.Critical(eventInfo, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Critical(string message, Exception exception) => _logger.Critical(LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Critical(LogEventInfo eventInfo, string message, Exception exception) => _logger.Critical(eventInfo, message, exception);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="message">event message</param>
        public void Warning(string message) => _logger.Warning(LogEventInfo.Empty, message);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Warning(LogEventInfo eventInfo, string message) => _logger.Warning(eventInfo, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Ignore(string message) => _logger.Ignore(LogEventInfo.Empty, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Ignore(LogEventInfo eventInfo, string message) => _logger.Ignore(eventInfo, message);

        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="logLevel">log level</param>
        /// <param name="logEventInfo">log event info</param>
        /// <param name="message">message</param>
        /// <param name="exception"></param>
        public void LogMessage(LogLevel logLevel, LogEventInfo logEventInfo, string message, Exception exception = null)
            => _logger.LogMessage(logLevel, logEventInfo, message, exception);

        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="logLevel">log level</param>
        /// <param name="logEventInfo">log event info</param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void LogMessage(LogLevel logLevel, LogEventInfo logEventInfo, object state, Exception exception, Func<object, Exception, string> formatter)
            => _logger.Log(logLevel, LogEventInfo.ToEventId(logEventInfo), state, exception, formatter);

        #endregion
    }

    public class LogEventWriter<T> : LogEventWriter
    {
        public LogEventWriter(Action<LogEventMessage> logEventListener) : base(typeof(T).FullName, logEventListener)
        {
        }

        public LogEventWriter(ILogEventReceiver receiver) : base(typeof(T).FullName, receiver)
        {
        }
    }

}
