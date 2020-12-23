using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.Collections;
using CafeLib.Core.Runnable;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Queueing
{
    /// <summary>
    /// Facilitates the producer/consumer queue.
    /// </summary>
    /// <typeparam name="T">queue item type</typeparam>
    public abstract class PriorityQueueProducer<T> : RunnerBase, IPriorityQueueProducer<T>
    {
        #region Private Members

        private readonly HashSet<IQueueConsumer<T>> _consumers;
        private readonly ReaderWriterPriorityQueue<T> _queue;

        #endregion

        #region Constructors

        /// <summary>
        /// PriorityQueueController constructor.
        /// </summary>
        /// <param name="frequency">producer frequency</param>
        protected PriorityQueueProducer(int frequency = default)
            : base(frequency)
        {
            _consumers = new HashSet<IQueueConsumer<T>>();
            _queue = new ReaderWriterPriorityQueue<T>();
        }

        /// <summary>
        /// PriorityQueueController constructor.
        /// </summary>
        /// <param name="consumer">consumer</param>
        /// <param name="frequency">producer frequency</param>
        protected PriorityQueueProducer(IQueueConsumer<T> consumer, int frequency = default)
            : this(frequency)
        {
            consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            Add(consumer);
        }

        /// <summary>
        /// PriorityQueueController constructor.
        /// </summary>
        /// <param name="consumers">collection of consumers</param>
        /// <param name="frequency">producer frequency</param>
        protected PriorityQueueProducer(IEnumerable<IQueueConsumer<T>> consumers, int frequency = default)
            : this(frequency)
        {
            consumers = consumers ?? throw new ArgumentNullException(nameof(consumers));
            _consumers = consumers.ToHashSet();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add consumer to producer.
        /// </summary>
        /// <param name="consumer"></param>
        public void Add(IQueueConsumer<T> consumer)
        {
            _consumers.Add(consumer);
        }

        /// <summary>
        /// Remove consumer from producer.
        /// </summary>
        /// <param name="consumer"></param>
        public void Remove(IQueueConsumer<T> consumer)
        {
            _consumers.Remove(consumer);
        }

        /// <summary>
        /// Produce a queued item.
        /// </summary>
        /// <param name="item">item to queue</param>
        public void Produce(T item)
        {
            Produce(item, 0);
        }

        /// <summary>
        /// Produce a queued item with priority.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Produce(T item, int priority)
        {
            _queue.Enqueue(item, priority);
        }

        /// <summary>
        /// Produce a queued item.
        /// </summary>
        /// <param name="item"></param>
        public void Produce(object item) => Produce((T)item, 0);

        /// <summary>
        /// Clears all queued items.
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Process queued items.
        /// </summary>
        /// <returns>awaitable task</returns>
        protected sealed override Task Run()
        {
            var item = _queue.Dequeue();
            var tasks = _consumers.Select(x => x.Consume(item));
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Stop the producer.
        /// </summary>
        /// <returns>awaitable task</returns>
        public override async Task Stop()
        {
            while (_queue.Any())
            {
                await Task.Delay(Delay);
            }

            await base.Stop();
        }

        #endregion

        #region IDisposible

        /// <summary>
        /// Dispose concurrent queue.
        /// </summary>
        /// <param name="disposing">indicate whether the queue is disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;
            Task.Run(async () =>
            {
                await Stop();
                _queue.Dispose();
                base.Dispose(true);
            });
        }

        #endregion
    }
}
