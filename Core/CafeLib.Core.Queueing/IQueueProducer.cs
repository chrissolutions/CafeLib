using CafeLib.Core.Runnable;

namespace CafeLib.Core.Queueing
{
    public interface IQueueProducer : IRunnable
    {
        /// <summary>
        /// Produce an item into the queue.
        /// </summary>
        /// <param name="item"></param>
        void Produce(object item);

        /// <summary>
        /// Clears all queued items.
        /// </summary>
        void Clear();
    }

    public interface IQueueProducer<in T> : IQueueProducer
    {
        /// <summary>
        /// Produce an item into the queue.
        /// </summary>
        /// <param name="item"></param>
        void Produce(T item);
    }
}
