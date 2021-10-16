using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace CafeLib.Core.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Info(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Information, LogEventInfo.Empty, message);

        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Info(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, LogLevel.Information, eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Error(this ILogger logger, string message)
            => LogMessage(logger, LogLevel.Error, LogEventInfo.Empty, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="exception">exception object</param>
        public static void Error(this ILogger logger, Exception exception)
            => LogMessage(logger, LogLevel.Error, LogEventInfo.Empty, exception.Message, exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Error(this ILogger logger, string message, Exception exception) 
            => LogMessage(logger, LogLevel.Error, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Error(this ILogger logger, LogEventInfo eventInfo, string message)
            => LogMessage(logger, LogLevel.Error, eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Error(this ILogger logger, LogEventInfo eventInfo, string message, Exception exception)
            => LogMessage(logger, LogLevel.Error, eventInfo, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Critical(this ILogger logger, string message) 
            => LogMessage(logger, LogLevel.Critical, LogEventInfo.Empty, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Critical(this ILogger logger, LogEventInfo eventInfo, string message)
            => LogMessage(logger, LogLevel.Critical, eventInfo, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="exception">exception object</param>
        public static void Critical(this ILogger logger, Exception exception)
            => LogMessage(logger, LogLevel.Critical, LogEventInfo.Empty, exception.Message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Critical(this ILogger logger, string message, Exception exception)
            => LogMessage(logger, LogLevel.Critical, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Critical(this ILogger logger, LogEventInfo eventInfo, string message, Exception exception) 
            => LogMessage(logger, LogLevel.Critical, eventInfo, message, exception);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">event message</param>
        public static void Warning(this ILogger logger, string message) 
            => LogMessage(logger, LogLevel.Warning, LogEventInfo.Empty, message);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Warning(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, LogLevel.Warning, eventInfo, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Ignore(this ILogger logger, string message) 
            => LogMessage(logger, LogLevel.None, LogEventInfo.Empty, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Ignore(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, LogLevel.None, eventInfo, message);

        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="logLevel">log level</param>
        /// <param name="logEventInfo">log event info</param>
        /// <param name="message">message</param>
        /// <param name="exception"></param>
        public static void LogMessage(this ILogger logger, LogLevel logLevel, LogEventInfo logEventInfo, string message, Exception exception = null)
        {
            logger.Log(logLevel, LogEventInfo.ToEventId(logEventInfo), message, exception, (state, ex) =>
            {
                // Set log message.
                var logMessage = state?.ToString() ?? string.Empty;

                // Set error message.
                var error = ex != null
                    ? $"{Environment.NewLine}exception: {ex.Message} {ex.InnerException?.Message}"
                    : string.Empty;

                // return log 
                return string.Format(CultureInfo.CurrentCulture, $"{logMessage}{error}");
            });
        }
    }
}
