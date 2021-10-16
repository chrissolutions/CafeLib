using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Collections.DirectedGraph
{
    /// <summary>
    ///	Abstract base class of all connection pin.
    /// </summary>
    internal class Pin
	{
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        private Pin() {}

        /// <summary>
        /// Constructs a pin for the owning vertex.
        /// </summary>
        /// <param name="owner"></param>
        public Pin(IVertex owner)
            : this()
        {
            Owner = owner;
            Edges = new EdgeCollection();
        }

        #endregion 

        #region Properties

        /// <summary>
        /// The vertex that owns the pin.
        /// </summary>
        protected IVertex Owner { get; }

        /// <summary>
        /// Gets the edge connections of the pin.
        /// </summary>
        public EdgeCollection Edges { get; }

        /// <summary>
        /// Gets the adjacent vertices of the pin.
        /// </summary>
        public IEnumerable<IVertex> AdjacentVertices
        {
            get { return Edges.Select(e => e.To); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determine whether the vertext is connected to the pin.
        /// </summary>
        /// <param name="vertex">vertex</param>
        /// <returns>
        ///     true : if connected.
        ///     false: if not connected.
        /// </returns>
        public bool IsConnected(IVertex vertex)
        {
            return Edges.Contains(vertex.Id);
        }

        /// <summary>
        /// Connect vertext to pin.
        /// </summary>
        /// <param name="vertex">vertex</param>
        public void Connect(IVertex vertex)
        {
            Edges.Add(new Edge(Owner, vertex));
        }

        /// <summary>
        /// Disconnect vertext from pin.
        /// </summary>
        /// <param name="vertex">vertex</param>
        public bool Disconnect(IVertex vertex)
        {
            return Edges.Remove(Edges[vertex]);
        }

        #endregion
    }
}
