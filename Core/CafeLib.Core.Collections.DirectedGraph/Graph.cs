using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Collections.DirectedGraph
{
	/// <summary>
	///	Graph structure.
	/// </summary>
    internal class Graph<T> : IGraph where T : IComparable<T>
    {
        private readonly VertexCollection<T> _vertices;
        private IVertex _origin;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Graph()
        {
            _vertices = new VertexCollection<T>();
        }

        /// <summary>
        /// Graph constructor with origin vertex.
        /// </summary>
        /// <param name="vertex">origin vertex</param>
        public Graph(IVertex vertex)
            : this()
        {
            Origin = vertex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves the number of graph verticies.
        /// </summary>
        public int Count => _vertices.Count;

	    /// <summary>
        /// Gets the origin vertex of the graph.
        /// </summary>
        public IVertex Origin
        {
            get => _origin;
            private set
            {
                if (_vertices.Contains(value))
                    _origin = value;
                else
                    throw new MissingMemberException("Vertex not in graph");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the graph enumerator.
        /// </summary>
        /// <returns>return vertex enumerator</returns>
        public IEnumerator<IVertex> GetEnumerator()
        {
            return _vertices.GetEnumerator();
        }

        /// <summary>
        /// Get the graph enumerator.
        /// </summary>
        /// <returns>return vertex enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a vertex to the graph.
        /// </summary>
        /// <param name="vertex">vertex to be added to the graph</param>
        public void Add(IVertex vertex)
	    {
            _vertices.Add((Vertex<T>)vertex);
            if (_vertices.Count == 1)
                Origin = vertex;
	    }

        /// <summary>
        /// Connects source and target verticies.
        /// </summary>
        /// <param name="source">source vertex</param>
        /// <param name="target">target vertex</param>
        /// <param name="weight">edge weight</param>
        /// <param name="content">edge content</param>
        /// <returns>the edges created by the connection</returns>
        public IEdges Connect(IVertex source, IVertex target, int weight = 0, object content = null)
	    {
            Add(source);
            Add(target);
            ((Vertex<T>)source).Attach((Vertex<T>)target);
            var edges = FindEdges(source, target);
            edges.Successor.Weight = edges.Predecessor.Weight = weight;
            edges.Successor.Content = edges.Predecessor.Content = content;
            return edges;
        }

        /// <summary>
        /// Returns whether the graph contains the vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns>
        ///     true: if vertex is in the graph
        ///     false: if vertex is not found in the graph.
        /// </returns>
        public bool Contains(IVertex vertex)
	    {
            return _vertices.Contains(vertex);
	    }

        /// <summary>
        /// Disconnnect target of edge from source.
        /// </summary>
        /// <param name="edge">edge</param>
        /// <returns>
        ///     true: if disconnect succeeded
        ///     false: disconnect failed.
        /// </returns>
        public bool Disconnect(IEdge edge)
	    {
            return ((Vertex<T>)edge.From).Detach(edge.To);
	    }

        /// <summary>
        /// Find the edge connecting the source & target verticies.
        /// </summary>
        /// <param name="source">source vertex</param>
        /// <param name="target">target vertex</param>
        /// <returns>edge</returns>
        public IEdges FindEdges(IVertex source, IVertex target)
        {
            return new Edges
                    {
                        Successor = ((Vertex<T>)source).SuccessorPin.Edges[target],
                        Predecessor = ((Vertex<T>)target).PredecessorPin.Edges[source]
                    };
        }

        /// <summary>
        /// Inserts a vertex into an edge.
        /// </summary>
        /// <param name="vertex">vertex to be inserted</param>
        /// <param name="edge">edge the vertex is inserted into</param>
        public void Insert(IVertex vertex, IEdge edge)
	    {
            var v = (Vertex<T>)vertex;
            var source = (Vertex<T>)edge.From;
            var target = (Vertex<T>)edge.To;

            _vertices.Add(v);
            source.Detach(target);
            source.Attach(v);
            v.Attach(target);
        }

        /// <summary>
        /// Create a vertex object.
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>vertex object</returns>
        public IVertex<T> NewVertex(T content)
	    {
            return new Vertex<T>(content);
	    }

        /// <summary>
        /// Removes vertex from graph.
        /// </summary>
        /// <param name="vertex">vertex</param>
	    public void Remove(IVertex vertex)
	    {
            var v = (Vertex<T>)vertex;
            v.Detach();
            _vertices.Remove(v);
            Origin = _vertices[0];
        }

        #endregion
    }
}
