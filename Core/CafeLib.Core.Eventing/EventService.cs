﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CafeLib.Core.Eventing.Subscribers;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.Eventing
{
    public class EventService : IEventService, IDisposable
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
        private static readonly object Mutex = new();

        /// <summary>
        /// Event broker name.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Event Service constructor.
        /// </summary>
        public EventService()
        {
            _subscriptions = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, EventSubscriber>>();
            _lookup = new ConcurrentDictionary<Guid, Type>();
            Name = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <param name='action'>Event action.</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public Guid Subscribe<T>(Action<T> action) where T : IEventMessage
        {
            lock (Mutex)
            {
                var subscribers = _subscriptions.GetOrAdd(typeof(T), new ConcurrentDictionary<Guid, EventSubscriber>());
                var subscriber = new ActionEventSubscriber<T>(action);
                subscribers.TryAdd(subscriber.Id, subscriber);
                _lookup.TryAdd(subscriber.Id, typeof(T));
                return subscriber.Id;
            }
        }

        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <param name='operation'>Event operation.</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public Guid Subscribe<T>(Func<T, Task> operation) where T : IEventMessage
        {
            lock (Mutex)
            {
                var subscribers = _subscriptions.GetOrAdd(typeof(T), new ConcurrentDictionary<Guid, EventSubscriber>());
                var subscriber = new TaskEventSubscriber<T>(operation);
                subscribers.TryAdd(subscriber.Id, subscriber);
                _lookup.TryAdd(subscriber.Id, typeof(T));
                return subscriber.Id;
            }
        }

        /// <summary>
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>Message.</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Publish<T>(T message) where T : IEventMessage
        {
            ConcurrentDictionary<Guid, EventSubscriber> subscribers;
            lock (Mutex)
            {
                if (!_subscriptions.ContainsKey(typeof(T))) return;
                subscribers = _subscriptions[typeof(T)];
            }

            subscribers.ForEachAsync(x => x.Value.Invoke(message).GetAwaiter().GetResult());
        }

        /// <summary>
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>Message.</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public Task PublishAsync<T>(T message) where T : IEventMessage
        {
            ConcurrentDictionary<Guid, EventSubscriber> subscribers;
            lock (Mutex)
            {
                if (!_subscriptions.ContainsKey(typeof(T))) return Task.CompletedTask;
                subscribers = _subscriptions[typeof(T)];
            }

            return subscribers.ForEachAsync(x => x.Value.Invoke(message));
        }

        /// <summary>
        /// Unsubscribe all specified handlers of type T.
        /// </summary>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>() where T : IEventMessage
        {
            lock (Mutex)
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
            lock (Mutex)
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
            lock (Mutex)
            {
                _subscriptions.ForEach(x => x.Value.Clear());
                _subscriptions.Clear();
                _lookup.Clear();
                _subscriptions = null;
                _lookup = null;
            }
        }
    }
}
