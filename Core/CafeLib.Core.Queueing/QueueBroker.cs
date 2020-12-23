using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Support;

namespace CafeLib.Core.Queueing
{
    public class QueueBroker : SingletonBase<QueueBroker>, IQueueBroker
    {
        /// <summary>
        /// This map contains the producers and its associated collection of consumers.
        /// </summary>
        private readonly ConcurrentDictionary<IQueueProducer, HashSet<IQueueConsumer>> _subscriptions;

        /// <summary>
        /// Synchronizes access to internal tables.
        /// </summary>
        private static readonly object Mutex = new object();

        /// <summary>
        /// EventBus constructor.
        /// </summary>
        private QueueBroker()
        {
            _subscriptions = new ConcurrentDictionary<IQueueProducer, HashSet<IQueueConsumer>>();
        }

        /// <summary>
        /// Register producer to broker.
        /// </summary>
        /// <param name="producer">producer</param>
        public void Register(IQueueProducer producer)
        {
            producer = producer ?? throw new ArgumentNullException();
            lock (Mutex)
            {
                _subscriptions.GetOrAdd(producer, new HashSet<IQueueConsumer>());
            }
        }

        /// <summary>
        /// Unregister producer from broker.
        /// </summary>
        /// <param name="producer"></param>
        public void Unregister(IQueueProducer producer)
        {
            producer = producer ?? throw new ArgumentNullException();
            lock (Mutex)
            {
                _subscriptions.TryRemove(producer, out _);
            }
        }

        /// <summary>
        /// Subscribe consumer to a producer..
        /// </summary>
        /// <param name="consumer">consumer</param>
        /// <param name="producer">producer</param>
        public void Subscribe(IQueueConsumer consumer, IQueueProducer producer)
        {
            consumer = consumer ?? throw new ArgumentNullException();
            producer = producer ?? throw new ArgumentNullException();

            lock (Mutex)
            {
                var consumers = _subscriptions.GetOrAdd(producer, new HashSet<IQueueConsumer>());
                consumers.Add(consumer);
            }
        }


        public void Unsubscribe(IQueueConsumer consumer, IQueueProducer producer)
        {
            consumer = consumer ?? throw new ArgumentNullException();
            producer = producer ?? throw new ArgumentNullException();

            lock (Mutex)
            {
                var consumers = _subscriptions.GetOrAdd(producer, new HashSet<IQueueConsumer>());
                consumers.Remove(consumer);
            }
        }

        internal IEnumerable<IQueueConsumer> GetConsumers(IQueueProducer producer)
        {
            producer = producer ?? throw new ArgumentNullException();

            lock (Mutex)
            {
                return _subscriptions.TryGetValue(producer, out var consumers)
                    ? consumers.ToArray()
                    : new IQueueConsumer[0];
            }
        }
    }
}
