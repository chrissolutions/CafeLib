using System;
using System.Collections.Concurrent;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;

namespace CafeLib.Core.Eventing
{
    public class EventService : SingletonBase<EventService>, IEventService, IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// This map contains the event Message type key and a collection of subscribers associated with the message type.
        /// </summary>
        private ConcurrentDictionary<Type, ConcurrentDictionary<Guid, EventSubscriber>> _subscriptions;

        /// <summary>
        /// This map provides type lookup.
        /// </summary>
        private ConcurrentDictionary<Guid, Type> _lookup;

        /// <summary>
        /// Synchronizes access to internal tables.
        /// </summary>
        private static readonly object _mutex = new object();

        /// <summary>
        /// EventBus constructor.
        /// </summary>
        private EventService()
        {
            _subscriptions = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, EventSubscriber>>();
            _lookup = new ConcurrentDictionary<Guid, Type>();
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
            lock (_mutex)
            {
                var subscribers = _subscriptions.GetOrAdd(typeof(T), new ConcurrentDictionary<Guid, EventSubscriber>());
                var subscriber = new EventSubscriber<T>(action);
                subscribers.TryAdd(subscriber.Id, subscriber);
                _lookup.TryAdd(subscriber.Id, typeof(T));
                return subscriber.Id;
            }
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
            ConcurrentDictionary<Guid, EventSubscriber> subscribers;
            lock (_mutex)
            {
                if (!_subscriptions.ContainsKey(typeof(T))) return;
                subscribers = _subscriptions[typeof(T)];
            }
            subscribers.ForEach(x => x.Value.Invoke(message));
        }

        /// <summary>
        /// Unsubscribe all specified handlers of type T.
        /// </summary>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>() where T : IEventMessage
        {
            lock (_mutex)
            {
                if (!_subscriptions.ContainsKey(typeof(T))) return;
                var subscribers = _subscriptions[typeof(T)];
                subscribers.ForEach(x =>
                {
                    var (key, _) = x;
                    subscribers.TryRemove(key, out _);
                    _lookup.TryRemove(key, out _);
                });
                _subscriptions.TryRemove(typeof(T), out _);
            }
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
            Unsubscribe(subscriberId);
        }

        /// <summary>
        /// Unsubscribe the specified handler using subscriber identifier.
        /// </summary>
        /// <param name="subscriberId">subscriber identifier</param>
        public void Unsubscribe(Guid subscriberId)
        {
            lock (_mutex)
            {
                if (!_lookup.ContainsKey(subscriberId)) return;
                var subscriberType = _lookup[subscriberId];

                var subscribers = _subscriptions[subscriberType];
                subscribers.TryRemove(subscriberId, out _);
                _lookup.TryRemove(subscriberId, out _);
                if (subscribers.Count == 0)
                {
                    _subscriptions.TryRemove(subscriberType, out _);
                }
            }
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
            lock (_mutex)
            {
                _subscriptions.ForEach(x => x.Value.Clear());
                _subscriptions.Clear();
                _lookup.Clear();
                _subscriptions = null;
                _lookup = null;
                ReleaseSingleton();
            }
        }
    }
}
