using System;
using CafeLib.Core.Eventing;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    public class LogEventMessage : EventMessage
    {
        #region Automatic Properties

        /// <summary>
        /// Log event category.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Error level.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Log event info.
        /// </summary>
        public LogEventInfo EventInfo { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public LogMessageInfo MessageInfo { get; }

        /// <summary>
        /// Error level.
        /// </summary>
        public Exception Exception { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// LogEventMessage default constructor.
        /// </summary>
        public LogEventMessage()
        {
            LogLevel = LogLevel.Information;
            Exception = null;
        }

        /// <summary>
        /// LogEventMessage constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="message">log message</param>
        /// <param name="eventInfo">log event info</param>
        public LogEventMessage(string category, string message, LogEventInfo eventInfo = default)
            : this(category, LogLevel.Information, eventInfo, message)
        {
        }

        /// <summary>
        /// LogEventMessage constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="logLevel">log level</param>
        /// <param name="message">log message</param>
        /// <param name="eventInfo">log event info</param>
        public LogEventMessage(string category, LogLevel logLevel, string message, LogEventInfo eventInfo = default)
            : this(category, logLevel, eventInfo, message)
        {
        }

        /// <summary>
        /// Log event message constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="exception">exception object</param>
        public LogEventMessage(string category, Exception exception, LogEventInfo eventInfo = default)
            : this(category, LogLevel.Error, eventInfo, null, exception)
        {
        }

        /// <summary>
        /// Log event message constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="logLevel">log level</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public LogEventMessage(string category, LogLevel logLevel, LogEventInfo eventInfo, string message, Exception exception = null)
            : this(category, logLevel, eventInfo, message, null, exception)    
        {
        }

        /// <summary>
        /// Log event message constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="logLevel">log level</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="messageInfo">log message info</param>
        /// <param name="exception">exception object</param>
        public LogEventMessage(string category, LogLevel logLevel, LogEventInfo eventInfo, string message, LogMessageInfo messageInfo, Exception exception = null)
        {
            Category = category;
            LogLevel = logLevel;
            EventInfo = eventInfo;
            Message = message ?? string.Empty;
            MessageInfo = messageInfo;
            Exception = exception;
        }

        #endregion
    }
}
