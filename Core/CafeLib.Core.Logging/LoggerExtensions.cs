using CafeLib.Core.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    public static class LoggerExtensions
    {
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
