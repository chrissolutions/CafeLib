namespace CafeLib.Core.Queueing
{
    public interface IPriorityQueueProducer<T> : IQueueProducer<T>
    {
        /// <summary>
        /// Produce an item into the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        void Produce(T item, int priority);
    }
}