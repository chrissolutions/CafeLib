///////////////////////////////////////////////////////////////////////////////
//	SearchAlgorithm.cs
//
//	Description:
//		Search object definition.
//
//	History:
//	18 Oct 2005 12:11PM		chriswa		Initial version.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace CafeLib.Core.Collections.Graph
{
    /// <summary>
    ///	Abstract base class for all search types.
    /// </summary>
    internal abstract class SearchAlgorithm : ISearch
	{
        #region Member_Variables

	    private readonly Dictionary<Guid, IVertex> _markers;

        #endregion

        #region Constructors

	    protected SearchAlgorithm()
	    {
	        _markers = new Dictionary<Guid, IVertex>();
	    }

        #endregion

        #region Properties

        /// <summary>
        /// Origin property.
        /// </summary>
        public IVertex Origin { get; set; }

        #endregion
        
        #region Methods

        /// <summary>
        /// Mark the vertex.
        /// </summary>
        /// <param name="v"></param>
        protected void Mark(IVertex v)
        {
        	if (!_markers.ContainsKey(v.Id))
        		_markers.Add(v.Id, v);
        }

        /// <summary>
        /// Unmark the vertex.
        /// </summary>
        /// <param name="v"></param>
        protected void Unmark(IVertex v)
        {
            if (_markers.ContainsKey(v.Id))
                _markers.Remove(v.Id);
        }

        protected bool IsMarked(IVertex v)
        {
            return _markers.ContainsKey(v.Id);
        }

        /// <summary>
        /// Clear the markers.
        /// </summary>
        protected virtual void Clear()
        {
            _markers.Clear();
        }

	    /// <summary>
	    /// Perform the search.
	    /// </summary>
	    public abstract void Search<T>(Func<IVertex<T>, IEdge, bool> visited) where T : IComparable<T>;

	    #endregion
	}
}
