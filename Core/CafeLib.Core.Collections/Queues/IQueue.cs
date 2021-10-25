using System.Collections.Generic;

namespace CafeLib.Core.Collections
{
    public interface IQueue<T> : IReadOnlyCollection<T>
    {
        void Clear();
        void Enqueue(T item);
        T Dequeue();
        bool TryDequeue(out T item);
        bool TryPeek(out T item);
    }
}
