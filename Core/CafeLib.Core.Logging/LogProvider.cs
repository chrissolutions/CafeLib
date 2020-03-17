using System;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    public class LogProvider<T> : ILoggerProvider where T : LoggerBase
    {
        #region Private Variables

        private readonly ILogEventReceiver _receiver;

        #endregion

        #region Automatic Properties

        public string Category { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="receiver">log event receiver</param>
        public LogProvider(ILogEventReceiver receiver)
        {
            _receiver = receiver;
        }

        /// <summary>
        /// LogProvider constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">log event receiver</param>
        internal LogProvider(NonNullable<string> category, ILogEventReceiver receiver)
        {
            Category = category.Value;
            _receiver = receiver;               
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
            Category ??= category;
            return (T)Activator.CreateInstance(typeof(T), Category, _receiver);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose() { }

        #endregion
    }
}
