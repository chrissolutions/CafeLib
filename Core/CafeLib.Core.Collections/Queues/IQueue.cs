using System.Collections.Generic;

namespace CafeLib.Core.Collections.Queues
{
    public interface IQueue<T> : IReadOnlyCollection<T>
    {
        void Clear();
        void Enqueue(T item);
        T Dequeue();
        bool TryDequeue(out T result);
        bool TryPeek(out T result);
    }
}
