using System;

namespace CafeLib.Core.Collections.Internal
{
    internal interface IRank<T> where T : IComparable<T>
    {
        int Priority { get; }
    }
}
