namespace CafeLib.Core.Queueing
{
    public interface IQueueProducer<T>
    {
        /// <summary>
        /// Add consumer to producer.
        /// </summary>
        /// <param name="consumer"></param>
        void Add(IQueueConsumer<T> consumer);

        /// <summary>
        /// Remove consumer from producer.
        /// </summary>
        /// <param name="consumer"></param>
        void Remove(IQueueConsumer<T> consumer);

        /// <summary>
        /// Produce an item into the queue.
        /// </summary>
        /// <param name="item"></param>
        void Produce(T item);
    }
}