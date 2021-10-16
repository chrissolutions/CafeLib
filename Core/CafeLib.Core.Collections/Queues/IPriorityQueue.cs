namespace CafeLib.Core.Collections
{
    public interface IPriorityQueue<T> : IQueue<T>
    {
        void Enqueue(T item, int priority);
    }
}
