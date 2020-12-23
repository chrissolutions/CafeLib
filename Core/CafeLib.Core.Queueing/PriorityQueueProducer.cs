using System;
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

        private readonly ReaderWriterPriorityQueue<T> _queue;

        #endregion

        #region Constructors

        /// <summary>
        /// QueueController constructor.
        /// </summary>
        /// <param name="frequency">producer frequency</param>
        protected PriorityQueueProducer(int frequency = default)
            : base(frequency)
        {
            QueueBroker.Current.Register(this);
            _queue = new ReaderWriterPriorityQueue<T>();
        }

        /// <summary>
        /// QueueController constructor.
        /// </summary>
        /// <param name="consumer">consumer</param>
        /// <param name="frequency">producer frequency</param>
        protected PriorityQueueProducer(IQueueConsumer<T> consumer, int frequency = default)
            : base(frequency)
        {
            consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            QueueBroker.Current.Subscribe(consumer, this);
            _queue = new ReaderWriterPriorityQueue<T>();
        }

        #endregion

        #region Methods

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
        public void Produce(object item) => Produce((T) item, 0);

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
            var tasks = QueueBroker.Current.GetConsumers(this).Select(x => x.Consume(item));
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
