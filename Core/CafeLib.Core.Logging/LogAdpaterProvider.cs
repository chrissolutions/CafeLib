using System;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    public class LogAdapterProvider<T> : ILoggerProvider where T : LogAdapter
    {
        #region Private Variables

        private readonly Action<LogEventMessage> _subscriber;

        #endregion

        #region Constructors

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="subscriber">log event message subscriber</param>
        public LogAdapterProvider(Action<LogEventMessage> subscriber = null)
        {
            _subscriber = subscriber ?? (x => { });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the logger.
        /// </summary>
        /// <param name="category">log category</param>
        /// <returns>logger</returns>
        public ILogger CreateLogger(string category)
        {
            return (T)Activator.CreateInstance(typeof(T), category, _subscriber);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose() { }

        #endregion
    }
}
