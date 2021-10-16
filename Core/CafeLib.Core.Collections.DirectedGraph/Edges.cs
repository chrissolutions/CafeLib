namespace CafeLib.Core.Collections.DirectedGraph
{
	/// <summary>
	///	Represents the graph edge allowing for edge content.
	/// </summary>
	internal class Edges : IEdges
    {
	    public IEdge Predecessor { get; set; }
	    public IEdge Successor { get; set; }
    }
}
