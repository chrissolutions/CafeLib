using CafeLib.Core.Support;
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
            => LogMessage(logger, ErrorLevel.Info, LogEventInfo.Empty, message);

        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Info(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, ErrorLevel.Info, eventInfo, message);

        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Ok(this ILogger logger, string message) 
            => LogMessage(logger, ErrorLevel.Ok, LogEventInfo.Empty, message);

        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Ok(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, ErrorLevel.Ok, eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Error(this ILogger logger, string message)
            => LogMessage(logger, ErrorLevel.Error, LogEventInfo.Empty, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="exception">exception object</param>
        public static void Error(this ILogger logger, Exception exception)
            => LogMessage(logger, ErrorLevel.Error, LogEventInfo.Empty, exception.Message, exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Error(this ILogger logger, string message, Exception exception) 
            => LogMessage(logger, ErrorLevel.Error, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Error(this ILogger logger, LogEventInfo eventInfo, string message)
            => LogMessage(logger, ErrorLevel.Error, eventInfo, message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Error(this ILogger logger, LogEventInfo eventInfo, string message, Exception exception)
            => LogMessage(logger, ErrorLevel.Error, eventInfo, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Critical(this ILogger logger, string message) 
            => LogMessage(logger, ErrorLevel.Critical, LogEventInfo.Empty, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Critical(this ILogger logger, LogEventInfo eventInfo, string message)
            => LogMessage(logger, ErrorLevel.Critical, eventInfo, message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="exception">exception object</param>
        public static void Critical(this ILogger logger, Exception exception)
            => LogMessage(logger, ErrorLevel.Critical, LogEventInfo.Empty, exception.Message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Critical(this ILogger logger, string message, Exception exception)
            => LogMessage(logger, ErrorLevel.Critical, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Critical(this ILogger logger, LogEventInfo eventInfo, string message, Exception exception) 
            => LogMessage(logger, ErrorLevel.Critical, eventInfo, message, exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Missing(this ILogger logger, string message)
            => LogMessage(logger, ErrorLevel.Missing, LogEventInfo.Empty, message);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="exception">exception object</param>
        public static void Missing(this ILogger logger, Exception exception)
            => LogMessage(logger, ErrorLevel.Missing, LogEventInfo.Empty, exception.Message, exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Missing(this ILogger logger, string message, Exception exception) 
            => LogMessage(logger, ErrorLevel.Missing, LogEventInfo.Empty, message, exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Missing(this ILogger logger, LogEventInfo eventInfo, string message)
            => LogMessage(logger,ErrorLevel.Missing, eventInfo, message);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public static void Missing(this ILogger logger, LogEventInfo eventInfo, string message, Exception exception) 
            => LogMessage(logger, ErrorLevel.Missing, eventInfo, message, exception);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">event message</param>
        public static void Warning(this ILogger logger, string message) 
            => LogMessage(logger, ErrorLevel.Warning, LogEventInfo.Empty, message);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Warning(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, ErrorLevel.Warning, eventInfo, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="message">log message</param>
        public static void Ignore(this ILogger logger, string message) 
            => LogMessage(logger, ErrorLevel.Ignore, LogEventInfo.Empty, message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        public static void Ignore(this ILogger logger, LogEventInfo eventInfo, string message) 
            => LogMessage(logger, ErrorLevel.Ignore, eventInfo, message);

        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="errorLevel">log level</param>
        /// <param name="logEventInfo">log event info</param>
        /// <param name="message">message</param>
        /// <param name="exception"></param>
        public static void LogMessage(this ILogger logger, ErrorLevel errorLevel, LogEventInfo logEventInfo, string message, Exception exception = null)
        {
            logger.Log(logger.ToLogLevel(errorLevel), LogEventInfo.ToEventId(logEventInfo), message, exception, (state, ex) =>
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

        /// <summary>
        /// Converts Micreosoft.Extensions.Logging.LogLevel to ErrorLevel.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="logLevel">log level</param>
        /// <returns>error level</returns>
        public static ErrorLevel ToErrorLevel(this ILogger logger, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return ErrorLevel.Critical;

                case LogLevel.Debug:
                case LogLevel.Trace:
                    return ErrorLevel.Diagnostic;

                case LogLevel.Error:
                    return ErrorLevel.Error;

                case LogLevel.Information:
                    return ErrorLevel.Info;

                case LogLevel.None:
                    return ErrorLevel.Ignore;

                case LogLevel.Warning:
                    return ErrorLevel.Warning;

                default:
                    return ErrorLevel.Ignore;
            }
        }

        /// <summary>
        /// Converts ErrorLevel to Microsoft.Extensions.Logging.LogLevel.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="errorLevel">error level</param>
        /// <returns>Microsoft.Extensions.Logging.LogLevel</returns>
        public static LogLevel ToLogLevel(this ILogger logger, ErrorLevel errorLevel)
        {
            switch (errorLevel)
            {
                case ErrorLevel.Critical:
                    return LogLevel.Critical;

                case ErrorLevel.Diagnostic:
                    return LogLevel.Debug;

                case ErrorLevel.Error:
                case ErrorLevel.Missing:
                    return LogLevel.Error;

                case ErrorLevel.Info:
                case ErrorLevel.Ok:
                    return LogLevel.Information;

                case ErrorLevel.Ignore:
                    return LogLevel.None;

                case ErrorLevel.Warning:
                    return LogLevel.Warning;

                default:
                    return LogLevel.None;
            }
        }
    }
}
