namespace CafeLib.Core.Collections.Queues
{
    public interface IPriorityQueue<T> : IQueue<T>
    {
        void Enqueue(T item, int priority);
    }
}
