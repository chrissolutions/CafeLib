using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CafeLib.Core.Eventing;

namespace CafeLib.Core.IoC
{
    internal class EventService : ServiceBase, IEventService
    {
        /// <summary>
        /// This map contains the event Message type key and a collection of subscribers associated with the message type.
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, object>> _subscriptions;

        /// <summary>
        /// EventBus constructor.
        /// </summary>
        public EventService()
        {
            _subscriptions = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, object>>();
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
            var subscribers = _subscriptions.GetOrAdd(typeof(T), new ConcurrentDictionary<Guid, object>());
            var key = Guid.NewGuid();
            subscribers.TryAdd(key, action);
            return key;
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
            if (_subscriptions.ContainsKey(typeof(T)))
            {
                var subscribers = _subscriptions[typeof(T)];
                foreach (var subscriber in subscribers)
                {
                    ((Action<T>)subscriber.Value)?.Invoke(message);
                }
            }
        }

        /// <summary>
        /// Unsubscribe all specified handlers of type T.
        /// </summary>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>() where T : IEventMessage
        {
            if (_subscriptions.ContainsKey(typeof(T)))
            {
                var subscribers = _subscriptions[typeof(T)];
                foreach (KeyValuePair<Guid, object> subscriber in subscribers)
                {
                    subscribers.TryRemove(subscriber.Key, out _);
                }

                _subscriptions.TryRemove(typeof(T), out _);
            }
        }

        /// <summary>
        /// Unsubscribe the specified handler of type T and Guid identifier.
        /// </summary>
        /// <param name="actionId"></param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>(Guid actionId) where T : IEventMessage
        {
            if (_subscriptions.ContainsKey(typeof(T)))
            {
                var subscribers = _subscriptions[typeof(T)];
                subscribers.TryRemove(actionId, out _);
                if (subscribers.Count == 0)
                {
                    _subscriptions.TryRemove(typeof(T), out _);
                }
            }
        }
    }
}
