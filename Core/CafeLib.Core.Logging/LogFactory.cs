using System;
using System.Collections.Concurrent;
using System.Linq;
using CafeLib.Core.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable StaticMemberInGenericType

namespace CafeLib.Core.Logging
{
    public class LogFactory<T> : ILoggerFactory where T : LogHandler
    {
        #region Private Variables

        private static readonly ConcurrentDictionary<string, ILogger> Loggers;
        private static readonly ConcurrentDictionary<string, ILoggerProvider> LoggerProviders;
        private readonly ILogEventMessenger _defaultMessenger;
        private bool _disposed;

        #endregion

        #region Constructors

        static LogFactory()
        {
            Loggers = new ConcurrentDictionary<string, ILogger>();
            LoggerProviders = new ConcurrentDictionary<string, ILoggerProvider>();
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="messenger">log event messenger</param>
        public LogFactory(ILogEventMessenger messenger = null)
            : this(new Guid().ToString(), messenger)
        {
        }

        /// <summary>
        /// CoreLoggerFactory constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="messenger">log event messenger</param>
        public LogFactory(NonNullable<string> category, ILogEventMessenger messenger)
        {
            _defaultMessenger = messenger;
            AddProvider(new LogProvider<T>(category, messenger));
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
            if (!LoggerProviders.TryGetValue(category, out var provider))
            {
                provider = new LogProvider<T>(category, _defaultMessenger);
                AddProvider(provider);
            }

            return Loggers.GetOrAdd(category, provider?.CreateLogger(category));
        }

        public void AddProvider(ILoggerProvider provider)
        {
            var category = (provider as LogProvider<T>)?.Category;
            LoggerProviders.GetOrAdd(category ?? throw new InvalidOperationException(nameof(category)), provider);
        }

        /// <summary>
        /// Disposes the factory.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            foreach (var provider in LoggerProviders.Select(x => x.Value))
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
