using System;
using System.Collections;

namespace CafeLib.Core.Collections.Graph
{
	/// <summary>
	///	Represents the graph edge allowing for edge content.
	/// </summary>
	internal class Edge : IEdge, IEqualityComparer
    {
        #region Constructors

        private Edge()
        { 
        }

	    /// <summary>
	    /// Edge constructor
	    /// </summary>
	    /// <param name="from">Source vertex</param>
	    /// <param name="to">Target vertex</param>
	    /// <param name="weight">Edge weight</param>
	    public Edge(IVertex from, IVertex to, int weight = 0)
            : this()
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            From = from;
            To = to;
            Weight = weight;
        }

        #endregion

        #region Properties

        public IVertex From { get; }

	    public IVertex To { get; }
	    public int Weight { get; set; }

        public object Content { get; set; }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            return Equals((Edge)obj);
        }

	    public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;

                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 ^ From.GetHashCode();
                hash = hash * 16777619 ^ To.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1">lvalue</param>
        /// <param name="e2">rvalue</param>
        /// <returns>
        ///     true: equal
        ///     false: not equal 
        /// </returns>
        public bool Equals(IEdge e1, IEdge e2)
        {
            return object.Equals(e1, e2);
        }

        /// <summary>
        /// GetHashCode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>hash code</returns>
        public int GetHashCode(IEdge obj)
        {
            return obj.GetHashCode();
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            return Equals((IEdge) x, (IEdge) y);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return GetHashCode((IEdge) obj);
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static bool operator ==(Edge e1, IEdge e2)
        {
            return ((object)e1) != null && e1.Equals(e2);
        }

	    public static bool operator !=(Edge e1, IEdge e2)
	    {
	        return ((object)e1) != null && !e1.Equals(e2);
	    }

        #endregion

        #region Helpers

        /// <summary>
        /// Edge equality.
        /// </summary>
        /// <param name="e">edge</param>
        /// <returns>
        ///     true: is equal.
        ///     false: not equal.
        /// </returns>
        private bool Equals(IEdge e)
        {
            return (From == e.From && To == e.To);
        }

        #endregion
    }
}
