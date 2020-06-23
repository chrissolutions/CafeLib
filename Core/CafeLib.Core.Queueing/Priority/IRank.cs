using System;

namespace CafeLib.Core.Queueing.Priority
{
    internal interface IRank<T> where T : IComparable<T>
    {
        int Priority { get; }
    }
}
