using System;

namespace CafeLib.Core.Collections.Graph
{
    /// <summary>
    ///	Defines graph vertex with content.
    /// </summary>
    public interface IVertex
    {
        Guid Id { get; }

        bool IsConnected { get; }

        IVertex Connect(IVertex vertex);

        bool Disconnect(IVertex vertex);
    }
}
