using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Collections.DirectedGraph
{
    /// <summary>
    ///	Performs depth-first search
    /// </summary>
    internal class DepthFirstSearch : SearchAlgorithm
    {
        #region Member_Variables

        private readonly Stack<IVertex> _visitedStack;

        #endregion

        #region Constructors

        /// <summary>
        /// DepthFirstSearch constructor for graph.
        /// </summary>
        /// <param name="graph">graph</param>
        public DepthFirstSearch(IGraph graph)
            : this(graph.Origin)
        {
        }

        /// <summary>
        /// DepthFirstSearch constructor with starting vertex.
        /// </summary>
        /// <param name="vertex">starting vertex</param>
        public DepthFirstSearch(IVertex vertex)
        {
            Origin = vertex;
            _visitedStack = new Stack<IVertex>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Search the graph.
        /// </summary>
        /// <typeparam name="T">content type</typeparam>
        /// <param name="predicate">match content callback predicate</param>
        public override void Search<T>(Func<IVertex<T>, IEdge, bool> predicate)
	    {
	        if (!(Origin is Vertex<T>))
	        {
	            throw new TypeAccessException("mismatched template type");    
	        }

            Clear();
            Mark(Origin);
	        if (predicate((IVertex<T>)Origin, null))
	            return;
            _visitedStack.Push(Origin);

            while (_visitedStack.Count != 0)
            {
                var source = (Vertex<T>)_visitedStack.Peek();
                var adjacentVertex = source.SuccessorPin.AdjacentVertices.FirstOrDefault(vertex => !IsMarked(vertex));
                if (adjacentVertex != null)
                {
                    Mark(adjacentVertex);
                    if (predicate((IVertex<T>)adjacentVertex, source.SuccessorPin.Edges[adjacentVertex]))
                    {
                        return;
                    }
                    _visitedStack.Push(adjacentVertex);
                }
                else
                {
                    _visitedStack.Pop();
                }
            }
        }

        #endregion
    }
}