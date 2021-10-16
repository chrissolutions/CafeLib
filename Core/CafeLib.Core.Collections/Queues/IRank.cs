using System;

namespace CafeLib.Core.Collections
{
    internal interface IRank<T> where T : IComparable<T>
    {
        int Priority { get; }
    }
}
