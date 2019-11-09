using System;
using System.Diagnostics;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    internal class LogProvider<T> : ILoggerProvider where T : LogHandler
    {
        #region Private Variables

        private readonly ILogEventMessenger _messenger;

        #endregion

        #region Automatic Properties

        public string Category { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// CoreLoggerProvider default constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="messenger">log event messenger</param>
        public LogProvider(NonNullable<string> category, ILogEventMessenger messenger)
        {
            Category = category.Value;
            _messenger = messenger;               
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
            Debug.Assert(category == Category, "category == Category");
            return (T)Activator.CreateInstance(typeof(T), Category, _messenger);
        }

        /// <summary>
        /// Disposes the provider.
        /// </summary>
        public void Dispose() { }

        #endregion
    }
}
