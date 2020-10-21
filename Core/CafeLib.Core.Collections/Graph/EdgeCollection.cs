using System;
using System.Collections.ObjectModel;

namespace CafeLib.Core.Collections.Graph
{
    /// <summary>
    /// Edge collection
    /// </summary>
    internal class EdgeCollection : KeyedCollection<Guid, IEdge>
	{
        #region Methods

        /// <summary>
        /// Add edge to the collection
        /// </summary>
        /// <param name="edge">edge</param>
        public new void Add(IEdge edge)
        {
            if (!Contains(edge))
                base.Add(edge);
        }

        /// <summary>
        /// Determine whether edge is contained in the collection.
        /// </summary>
        /// <param name="edge">edge</param>
        /// <returns>
        ///     true: if edge is contained in the collection.
        ///     false: edge is not contained in the collection.
        /// </returns>
        public new bool Contains(IEdge edge)
        {
            return Contains(edge.To.Id);
        }

        /// <summary>
        /// Determine whether the vertex is contained in the collection.
        /// </summary>
        /// <param name="vertex">vertex</param>
        /// <returns>
        ///     true: if vertex is contained in the collection.
        ///     false: vertex is not contained in the collection.
        /// </returns>
        public bool Contains(IVertex vertex)
        {
            return Contains(vertex.Id);
        }

        /// <summary>
        /// Removes the edge from the collection.
        /// </summary>
        /// <param name="edge">edge</param>
        public new bool Remove(IEdge edge)
        {
            return Contains(edge) && base.Remove(edge);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the key for the edge item
        /// </summary>
        /// <param name="edge">edge</param>
        /// <returns>the key representing the edge</returns>
        protected override Guid GetKeyForItem(IEdge edge)
        {
            return edge.To.Id;
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Edge collection indexer.
        /// </summary>
        /// <param name="vertex">vertex</param>
        /// <returns>find the edge associated with the vertex</returns>
        public IEdge this[IVertex vertex] => base[vertex.Id];

        #endregion
	}
}
