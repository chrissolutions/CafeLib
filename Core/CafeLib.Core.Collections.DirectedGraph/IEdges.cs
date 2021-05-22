namespace CafeLib.Core.Collections.DirectedGraph
{
	/// <summary>
	///	Represents the graph edge allowing for edge content.
	/// </summary>
	public interface IEdges
    {
        IEdge Predecessor { get; }
        IEdge Successor { get; }
    }
}
