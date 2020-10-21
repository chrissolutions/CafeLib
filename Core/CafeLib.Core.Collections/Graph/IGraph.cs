using System.Collections.Generic;
namespace CafeLib.Core.Collections.Graph
{
	/// <summary>
	///	Contains reference to the node value and is the vertex inner object.
	/// </summary>
    public interface IGraph : IEnumerable<IVertex>
	{
        int Count { get; }

        IVertex Origin { get; }

        void Add(IVertex v);

        IEdges Connect(IVertex source, IVertex target, int weight=0, object content = null);

        bool Contains(IVertex v);

        bool Disconnect(IEdge e);

        IEdges FindEdges(IVertex source, IVertex target);

        void Insert(IVertex v, IEdge e);

        void Remove(IVertex v);
	}
}
