using System;
using System.Collections.Concurrent;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;

namespace CafeLib.Core.Eventing
{
    public class EventService : SingletonBase<EventService>, IEventService, IDisposable
    {
        private bool _disposed;
        private readonly EventBroker _serviceBroker;
        private readonly ConcurrentDictionary<string, IEventBroker> _brokers;

        /// <summary>
        /// EventBus constructor.
        /// </summary>
        private EventService()
        {
            _brokers = new ConcurrentDictionary<string, IEventBroker>();
            _serviceBroker = new EventBroker();
            _brokers.TryAdd(_serviceBroker.Name, _serviceBroker);
        }

        /// <summary>
        /// Create a new event broker.
        /// </summary>
        /// <returns></returns>
        public IEventBroker CreateBroker(string name)
        {
            return _brokers.GetOrAdd(name, new EventBroker());
        }

        /// <summary>
        /// Get an event broker.
        /// </summary>
        /// <param name="name">event broker name</param>
        /// <returns>event broker</returns>
        public IEventBroker GetBroker(string name)
        {
            return _brokers.TryGetValue(name, out var value) ? value : null;
        }

        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <param name='action'>
        /// Event action.
        /// </param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public Guid Subscribe<T>(Action<T> action) where T : IEventMessage
        {
            return _serviceBroker.Subscribe(action);
        }

        /// <summary>
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Publish<T>(T message) where T : IEventMessage
        {
            _serviceBroker.Publish(message);
        }

        /// <summary>
        /// Unsubscribe all specified handlers of type T.
        /// </summary>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>() where T : IEventMessage
        {
            _serviceBroker.Unsubscribe<T>();
        }

        /// <summary>
        /// Unsubscribe the specified handler of type T and Guid identifier.
        /// </summary>
        /// <param name="subscriberId">subscriber identifier</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>(Guid subscriberId) where T : IEventMessage
        {
            _serviceBroker.Unsubscribe(subscriberId);
        }

        /// <summary>
        /// Unsubscribe the specified handler using subscriber identifier.
        /// </summary>
        /// <param name="subscriberId">subscriber identifier</param>
        public void Unsubscribe(Guid subscriberId)
        {
            _serviceBroker.Unsubscribe(subscriberId);
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
