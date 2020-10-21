using System;

namespace CafeLib.Core.Collections.Graph
{
    /// <summary>
    ///	Defines graph vertex with content.
    /// </summary>
    public interface IVertex<T> : IVertex where T : IComparable<T>
    {
        T Content { get; set; }
    }
}
