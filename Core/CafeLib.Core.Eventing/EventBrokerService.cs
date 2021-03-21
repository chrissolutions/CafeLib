using System;
using System.Collections.Concurrent;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;

namespace CafeLib.Core.Eventing
{
    public class EventBrokerService : SingletonBase<EventBrokerService>, IEventBrokerService, IDisposable
    {
        private bool _disposed;
        private readonly ConcurrentDictionary<string, IEventBroker> _brokers;

        /// <summary>
        /// EventBus constructor.
        /// </summary>
        private EventBrokerService()
        {
            _brokers = new ConcurrentDictionary<string, IEventBroker>();
        }

        /// <summary>
        /// Create a new event broker.
        /// </summary>
        /// <returns>new event broker</returns>
        public IEventBroker CreateEventBroker(string name)
        {
            return _brokers.GetOrAdd(name, new EventBroker(name));
        }

        public IEventBroker GetEventBroker(string name)
        {
            return _brokers.TryGetValue(name, out var value) ? value : null;
        }

        /// <summary>
        /// Dispose event service.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose event service resources.
        /// </summary>
        /// <param name="disposing">flag indicating disposing</param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _brokers.Values.ForEach(x => ((EventBroker)x).Dispose());
            ReleaseSingleton();
        }
    }
}
