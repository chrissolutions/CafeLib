namespace CafeLib.Core.Collections.DirectedGraph
{
	/// <summary>
	///	Represents the graph edge allowing for edge content.
	/// </summary>
	public interface IEdge
    {
        IVertex From { get; }
        IVertex To { get; }
        int Weight { get; set; }
        object Content { get; set; }
    }
}
