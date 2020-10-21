using System;

namespace CafeLib.Core.Collections.Graph
{
	/// <summary>
	///	Abstract base class for all search types.
	/// </summary>
    internal interface ISearch
	{
        IVertex Origin { get; }

        void Search<T>(Func<IVertex<T>, IEdge, bool> predicate) where T : IComparable<T>;
	}
}
