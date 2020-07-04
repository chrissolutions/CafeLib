using System;
using System.Globalization;
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
            : base(category, new LogEventReceiver(logEventListener))
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
        public void Info(string message) => LogMessage(ErrorLevel.Info, LogEventInfo.Empty, message);

        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Info(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Info, eventInfo, message);

        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="message">log message</param>
        public void Ok(string message) => LogMessage(ErrorLevel.Ok, LogEventInfo.Empty, message);

        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Ok(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Ok, eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Error(string message) => LogMessage(ErrorLevel.Error, LogEventInfo.Empty, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Error(string message, Exception exception) => LogMessage(ErrorLevel.Error, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Error(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Error, eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Error(LogEventInfo eventInfo, string message, Exception exception) => LogMessage(ErrorLevel.Error, eventInfo, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Critical(string message) => LogMessage(ErrorLevel.Critical, LogEventInfo.Empty, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Critical(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Critical, eventInfo, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Critical(string message, Exception exception) => LogMessage(ErrorLevel.Critical, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Critical(LogEventInfo eventInfo, string message, Exception exception) => LogMessage(ErrorLevel.Critical, eventInfo, message, exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Missing(string message) => LogMessage(ErrorLevel.Missing, LogEventInfo.Empty, message);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Missing(string message, Exception exception) => LogMessage(ErrorLevel.Missing, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Missing(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Missing, eventInfo, message);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public void Missing(LogEventInfo eventInfo, string message, Exception exception) => LogMessage(ErrorLevel.Missing, eventInfo, message, exception);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="message">event message</param>
        public void Warning(string message) => LogMessage(ErrorLevel.Warning, LogEventInfo.Empty, message);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Warning(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Warning, eventInfo, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="message">log message</param>
        public void Ignore(string message) => LogMessage(ErrorLevel.Ignore, LogEventInfo.Empty, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public void Ignore(LogEventInfo eventInfo, string message) => LogMessage(ErrorLevel.Ignore, eventInfo, message);

        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="errorLevel">log level</param>
        /// <param name="logEventInfo">log event info</param>
        /// <param name="message">message</param>
        /// <param name="exception"></param>
        public void LogMessage(ErrorLevel errorLevel, LogEventInfo logEventInfo, NonNullable<string> message, Exception exception = null)
        {
            _logger.Log(_logger.ToLogLevel(errorLevel), LogEventInfo.ToEventId(logEventInfo), message.Value, exception, LogFormatter);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Logging formatter.
        /// </summary>
        /// <param name="state">state object</param>
        /// <param name="exception">exception object</param>
        /// <returns></returns>
        protected virtual string LogFormatter(object state, Exception exception)
        {
            // Set log message.
            var message = state?.ToString() ?? string.Empty;

            // Set error message.
            var error = exception != null
                ? $"{Environment.NewLine}exception: {exception.Message} {exception.InnerException?.Message}"
                : string.Empty;

            // return log 
            return string.Format(CultureInfo.CurrentCulture, $"{message}{error}");
        }

        #endregion
    }
}
