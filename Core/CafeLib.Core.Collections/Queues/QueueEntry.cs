using System;

namespace CafeLib.Core.Collections.Queues
{
    internal struct QueueEntry<T> : IRank<QueueEntry<T>>, IComparable<QueueEntry<T>>
    {
        readonly DateTime _creationTime;
        private readonly bool _fifo;

        public QueueEntry(T value, int priority, bool fifo = true)
        {
            _creationTime = DateTime.UtcNow;
            _fifo = fifo;
            Value = value;
            Priority = priority;
        }

        public T Value { get; }

        public int Priority { get; }

        public int CompareTo(QueueEntry<T> other)
        {
            int pri = Priority.CompareTo(other.Priority);
            pri = (pri != 0)
                ? pri
                : (_fifo)
                    ? other._creationTime.CompareTo(_creationTime)
                    : _creationTime.CompareTo(other._creationTime);
            return pri;
        }

        public override string ToString()
        {
            return $"[{Priority} : {_creationTime.TimeOfDay.TotalMilliseconds}] {Value}";
        }
    }
}