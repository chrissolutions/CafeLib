using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Queueing
{
    /// <summary>
    /// Consumer base class.
    /// </summary>
    /// <typeparam name="T">queue item type</typeparam>
    public abstract class QueueConsumer<T> : IQueueConsumer<T>
    {
        public abstract Task Consume(T item);

        public Task Consume(object item) => Consume((T)item);
    }
}