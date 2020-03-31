using CafeLib.Core.Data;
using CafeLib.Data.Sources;

namespace CafeLib.Data.Persistence
{
    /// <summary>
    /// Database storage wrapper class.
    /// </summary>
    public class Storage<T> : StorageBase where T : Domain, new()
    {
        /// <summary>
        /// Constructs a storage from the connection uri.
        /// </summary>
        /// <param name="connectionUri"></param>
        /// <param name="options">storage options</param>
        public Storage(string connectionUri, IConnectionOptions options)
            : this(new StorageConnectionInfo(connectionUri, new T(), options))
        {
        }

        /// <summary>
        /// Constructs a storage from the connection uri.
        /// </summary>
        /// <param name="connectionInfo"></param>
        private Storage(IConnectionInfo connectionInfo)
            : base(connectionInfo)
        {
        }
    }
}