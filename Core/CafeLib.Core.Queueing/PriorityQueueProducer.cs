using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using CafeLib.Core.Collections.Queues;
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

        private readonly IQueueConsumer<T> _queueConsumer;
        private readonly ReaderWriterPriorityQueue<T> _queue;

        #endregion

        #region Constructors

        /// <summary>
        /// QueueController constructor.
        /// </summary>
        /// <param name="queueConsumer">consumer queue</param>
        /// <param name="frequency">producer frequency</param>
        protected PriorityQueueProducer(IQueueConsumer<T> queueConsumer, int frequency = default)
            : base(frequency)
        {
            Contract.Assert(queueConsumer != null, nameof(queueConsumer));
            _queueConsumer = queueConsumer;
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
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Produce(T item, int priority)
        {
            _queue.Enqueue(item, priority);
        }

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
        protected sealed override async Task Run()
        {
            await _queueConsumer.Consume(_queue.Dequeue());
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
