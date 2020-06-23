using CafeLib.Core.Runnable;

namespace CafeLib.Core.Queueing
{
    public interface IPriorityQueueProducer<in T> : IQueueProducer<T>
    {
        /// <summary>
        /// Produce an item into the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        void Produce(T item, int priority);
    }
}
