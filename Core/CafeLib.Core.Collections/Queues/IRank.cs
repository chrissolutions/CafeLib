using System;

namespace CafeLib.Core.Collections.Queues
{
    internal interface IRank<T> where T : IComparable<T>
    {
        int Priority { get; }
    }
}
