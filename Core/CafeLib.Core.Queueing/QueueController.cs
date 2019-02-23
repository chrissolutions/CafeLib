using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using CafeLib.Core.Async;
using CafeLib.Core.Runnable;

namespace CafeLib.Core.Queueing
{
    /// <summary>
    /// Facilitates the producer/consumer queue.
    /// </summary>
    /// <typeparam name="T">queue item type</typeparam>
    public abstract class QueueController<T> : Runner, IQueueProducer<T>
    {
        #region Private Members

        private readonly IQueueConsumer<T> _queueConsumer;
        private readonly QueueCore<T> _queue;

        #endregion

        #region Constructors

        /// <summary>
        /// QueueController constructor.
        /// </summary>
        /// <param name="queueConsumer">consumer queue</param>
        protected QueueController(IQueueConsumer<T> queueConsumer)
        {
            Contract.Assert(queueConsumer != null, nameof(queueConsumer));
            _queueConsumer = queueConsumer;
            _queue = new QueueCore<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Produce a queued item.
        /// </summary>
        /// <param name="item">item to queue</param>
        public void Produce(T item)
        {
            _queue.Enqueue(item);
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
        protected override async Task Run()
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
            AsyncTask.Run(Stop);
            _queue.Dispose();
            base.Dispose(true);
        }

        #endregion
    }
}
