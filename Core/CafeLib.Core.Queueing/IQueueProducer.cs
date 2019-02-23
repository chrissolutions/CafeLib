using CafeLib.Core.Runnable;

namespace CafeLib.Core.Queueing
{
    public interface IQueueProducer<in T> : IRunnable
    {
        /// <summary>
        /// Produce an item into the queue.
        /// </summary>
        /// <param name="item"></param>
        void Produce(T item);

        /// <summary>
        /// Clears all queued items.
        /// </summary>
        void Clear();
    }
}
