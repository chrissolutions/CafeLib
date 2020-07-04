using System;
using System.Collections.Concurrent;
using System.Linq;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.Logging;
// ReSharper disable StaticMemberInGenericType

namespace CafeLib.Core.Logging
{
    public class LogFactory<T> : ILoggerFactory where T : ILogger
    {
        #region Private Variables

        private static readonly ConcurrentDictionary<string, ILogger> _loggers;
        private static readonly ConcurrentDictionary<string, ILoggerProvider> _loggerProviders;
        private readonly ILogEventReceiver _receiver;
        private bool _disposed;

        #endregion

        #region Constructors

        static LogFactory()
        {
            _loggers = new ConcurrentDictionary<string, ILogger>();
            _loggerProviders = new ConcurrentDictionary<string, ILoggerProvider>();
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        public LogFactory()
            : this(new Guid().ToString(), new LogEventReceiver())
        {
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="receiver">log event receiver</param>
        public LogFactory(ILogEventReceiver receiver)
            : this(new Guid().ToString(), receiver)
        {
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">log event receiver</param>
        public LogFactory(NonNullable<string> category, ILogEventReceiver receiver)
        {
            _receiver = receiver ?? new LogEventReceiver();
            AddProvider(new LogProvider<T>(category, new NonNullable<ILogEventReceiver>(_receiver)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a logger.
        /// </summary>
        /// <param name="category">log category</param>
        /// <returns>logger</returns>
        public ILogger CreateLogger(string category)
        {
            if (!_loggerProviders.TryGetValue(category, out var provider))
            {
                provider = new LogProvider<T>(category, new NonNullable<ILogEventReceiver>(_receiver));
                AddProvider(provider);
            }

            return _loggers.GetOrAdd(category, provider?.CreateLogger(category));
        }

        public void AddProvider(ILoggerProvider provider)
        {
            var category = (provider as LogProvider<T>)?.Category;
            _loggerProviders.GetOrAdd(category ?? throw new InvalidOperationException(nameof(category)), provider);
        }

        /// <summary>
        /// Disposes the factory.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            foreach (var provider in _loggerProviders.Select(x => x.Value))
            {
                try
                {
                    provider.Dispose();
                }
                catch
                {
                    // ignore.
                }
            }

            _disposed = true;
        }

        #endregion 
    }
}
