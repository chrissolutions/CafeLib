using System;
using System.Collections.ObjectModel;

namespace CafeLib.Core.Collections.Graph
{
    /// <summary>
    ///	Collection of vertices.
    /// </summary>
    internal class VertexCollection<T> : KeyedCollection<Guid, Vertex<T>> where T : IComparable<T>
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public new void Add(Vertex<T> v)
        {
            if (!Contains(v))
                base.Add(v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public new bool Contains(Vertex<T> v)
        {
            return Contains(v.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public new bool Remove(Vertex<T> v)
        {
            return Contains(v) && Remove(v.Id);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected override Guid GetKeyForItem(Vertex<T> v)
        {
            return v.Id;
        }

        #endregion
    }
}