using System;
using CafeLib.Core.Eventing;
using CafeLib.Core.Support;

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
        public ErrorLevel ErrorLevel { get; }

        /// <summary>
        /// Log event info.
        /// </summary>
        public LogEventInfo EventInfo { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public string Message { get; }

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
            ErrorLevel = ErrorLevel.Info;
            Exception = null;
        }

        /// <summary>
        /// LogEventMessage constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="message">log message</param>
        /// <param name="eventInfo">log event info</param>
        public LogEventMessage(string category, string message, LogEventInfo eventInfo = default(LogEventInfo))
            : this(category, ErrorLevel.Info, eventInfo, message)
        {
        }

        /// <summary>
        /// LogEventMessage constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="errorLevel">error level</param>
        /// <param name="message">log message</param>
        /// <param name="eventInfo">log event info</param>
        public LogEventMessage(string category, ErrorLevel errorLevel, string message, LogEventInfo eventInfo = default(LogEventInfo))
            : this(category, errorLevel, eventInfo, message)
        {
        }

        /// <summary>
        /// Log event message constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="exception">exception object</param>
        public LogEventMessage(string category, Exception exception, LogEventInfo eventInfo = default(LogEventInfo))
            : this(category, ErrorLevel.Error, eventInfo, null, exception)
        {
        }

        /// <summary>
        /// Log event message constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="errorLevel">error level</param>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        public LogEventMessage(string category, ErrorLevel errorLevel, LogEventInfo eventInfo, string message, Exception exception = null)
        {
            Category = category;
            ErrorLevel = errorLevel;
            EventInfo = eventInfo;
            Message = message ?? string.Empty;
            Exception = exception;
        }

        #endregion
    }
}
