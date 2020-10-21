using System;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Support;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Collections.Graph
{
    /// <summary>
    ///	Defines graph vertex with content.
    /// </summary>
    /// <typeparam name="T">content type</typeparam>
    public class Vertex<T> : IVertex<T> where T : IComparable<T>
    {
        #region Member_Variables

        private KeyValuePair<Guid, T> _node;

        #endregion

        #region Constructors

        public Vertex()
            : this(ObjectId.GenerateNewId(), default)
        { 
        }

        public Vertex(T content)
            : this(ObjectId.GenerateNewId(), content)
        {
        }

        private Vertex(Guid key, T content)
        {
            _node = new KeyValuePair<Guid, T>(key, content);
            PredecessorPin = new Pin(this);
            SuccessorPin = new Pin(this);
        }

        #endregion

        #region Properties

        public Guid Id => _node.Key;

        /// <summary>
        /// Determines whether the vertex has a connection.
        /// </summary>
        public bool IsConnected => (PredecessorPin.Edges.Count != 0 || SuccessorPin.Edges.Count != 0);

        /// <summary>
        /// Gets and sets the vertex content.
        /// </summary>
        public T Content
        {
            get => _node.Value;
            set => _node = new KeyValuePair<Guid, T>(_node.Key, value);
        }

        /// <summary>
        /// Gets the predecessor pin.
        /// </summary>
        internal Pin PredecessorPin { get; }

        /// <summary>
        /// Gets the successor pin.
        /// </summary>
        internal Pin SuccessorPin { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Connect target vertex to source.
        /// </summary>
        /// <param name="vertex">target vertex to connect to source.</param>
        /// <returns>the current vertex</returns>
        public IVertex Connect(IVertex vertex)
        {
            Attach((Vertex<T>)vertex);
            return vertex;
        }

        /// <summary>
        /// Disconnect target vertex from source.
        /// </summary>
        /// <param name="vertex">target vertex to disconnect from source.</param>
        /// <returns>
        ///     true: successfully disconnect target.
        ///     false: target not found or not connected.
        /// </returns>
        public bool Disconnect(IVertex vertex)
        {
            return Detach(vertex);
        }

        /// <summary>
        /// Attach vertex to source.
        /// </summary>
        /// <param name="v">vertex to attach</param>
        internal void Attach(IVertex v)
        {
            SuccessorPin.Connect(v);
            ((Vertex<T>)v).PredecessorPin.Connect(this);
        }

        /// <summary>
        /// Detach vertex from source.
        /// </summary>
        /// <param name="vertex">vertex to detach from source.</param>
        /// <returns>
        ///     true: successfully detached vertex from source.
        ///     false: vertex not found or not connected.
        /// </returns>
        internal bool Detach(IVertex vertex)
        {
            return SuccessorPin.Disconnect(vertex) || ((Vertex<T>)vertex).PredecessorPin.Disconnect(this);
        }

        /// <summary>
        /// Detach all vertices.
        /// </summary>
        /// <returns>
        ///     true: successfully detached the vertex
        ///     false: detach unsuccessful.
        /// </returns>
        internal bool Detach()
        {
            IEnumerable<IVertex> adjacentVertices = new List<IVertex>(SuccessorPin.AdjacentVertices);
            var result = adjacentVertices.Aggregate(true, (current, vertex) => current & Detach(vertex));

            adjacentVertices = new List<IVertex>(PredecessorPin.AdjacentVertices);
            return adjacentVertices.Aggregate(result, (current, vertex) => current & ((Vertex<T>)vertex).Detach(this));
        }

        /// <summary>
        /// Return the successor edge for the vertex.
        /// </summary>
        /// <param name="vertex">target vertex</param>
        /// <returns>edge associated with target vertex or null</returns>
        internal IEdge SuccessorEdge(IVertex vertex)
        {
            return SuccessorPin.Edges[vertex];
        }

        /// <summary>
        /// Return the predecessor edge for the vertex.
        /// </summary>
        /// <param name="v">target vertex</param>
        /// <returns>edge associated with target vertex or null</returns>
        internal IEdge PredecessorEdge(IVertex v)
        {
            return PredecessorPin.Edges[v];
        }

        /// <summary>
        /// Convert vertex to string.
        /// </summary>
        /// <returns>vertex string representation</returns>
        public override string ToString()
        {
            return Content.ToString();
        }

        #endregion
    }
}
