using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Collections.DirectedGraph
{
    /// <summary>
    ///	Performs breadth-first search
    /// </summary>
    internal class BreadthFirstSearch : SearchAlgorithm
    {
        #region Member_Variables

        private readonly Queue<IVertex> _visitedQueue;

        #endregion

        #region Constructors

        public BreadthFirstSearch(IGraph g)
            : this(g.Origin)
        {
        }

        public BreadthFirstSearch(IVertex v)
        {
            Origin = v;
            _visitedQueue = new Queue<IVertex>();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override void Search<T>(Func<IVertex<T>, IEdge, bool> predicate)
        {
            Clear();
            Mark(Origin);
            if (predicate((IVertex<T>)Origin, null))
                return;
            _visitedQueue.Enqueue(Origin);

            while (_visitedQueue.Count != 0)
            {
                var source = (Vertex<T>)_visitedQueue.Dequeue();
                foreach (var adjacentVertex in source.SuccessorPin.AdjacentVertices.Where(vertex => !IsMarked(vertex)))
                {
                    Mark(adjacentVertex);
                    if (predicate((IVertex<T>) adjacentVertex, source.SuccessorPin.Edges[adjacentVertex]))
                    {
                        return;
                    }
                    _visitedQueue.Enqueue(adjacentVertex);
                }
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _visitedQueue.Clear();
        }

        #endregion
    }
}