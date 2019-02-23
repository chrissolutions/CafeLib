using System;

namespace CafeLib.Core.Logging
{
    /// <summary>
    /// Logs event by classification.
    /// </summary>
    public interface ILogEventWriter
    {
        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="message">log message</param>
        void Info(string message);

        /// <summary>
        /// Log info event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Info(LogEventInfo eventInfo, string message);

        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="message">log message</param>
        void Ok(string message);

        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Ok(LogEventInfo eventInfo, string message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="message">log message</param>
        void Error(string message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Error(LogEventInfo eventInfo, string message);

        /// <summary>
        /// Log error event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        void Error(LogEventInfo eventInfo, string message, Exception exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="message">log message</param>
        void Critical(string message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Critical(LogEventInfo eventInfo, string message);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        void Critical(string message, Exception exception);

        /// <summary>
        /// Log critical event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        void Critical(LogEventInfo eventInfo, string message, Exception exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="message">log message</param>
        void Missing(string message);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        void Missing(string message, Exception exception);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Missing(LogEventInfo eventInfo, string message);

        /// <summary>
        /// Log missing event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        /// <param name="exception">exception object</param>
        void Missing(LogEventInfo eventInfo, string message, Exception exception);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="message">event message</param>
        void Warning(string message);

        /// <summary>
        /// Log warning event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Warning(LogEventInfo eventInfo, string message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="message">log message</param>
        void Ignore(string message);

        /// <summary>
        /// Log ignore event.
        /// </summary>
        /// <param name="eventInfo">log event info</param>
        /// <param name="message">log message</param>
        void Ignore(LogEventInfo eventInfo, string message);
    }
}
