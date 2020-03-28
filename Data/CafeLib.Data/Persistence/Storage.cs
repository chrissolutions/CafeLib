using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Data.Options;

namespace CafeLib.Data.Persistence
{
    /// <summary>
    /// Database storage wrapper class.
    /// </summary>
    public class Storage<T> : StorageBase where T : Domain
    {
        /// <summary>
        /// Context.
        /// </summary>
        protected new T Domain { get; }

        /// <summary>
        /// Constructs a storage from the connection uri.
        /// </summary>
        /// <param name="connectionUri"></param>
        /// <param name="options">storage options</param>
        public Storage(string connectionUri, IConnectionOptions? options = null)
            : this(new StorageConnectionInfo(connectionUri, options))
        {
        }

        /// <summary>
        /// Constructs a storage from the connection uri.
        /// </summary>
        /// <param name="connectionInfo"></param>
        private Storage(IConnectionInfo connectionInfo)
            : base(connectionInfo, typeof(T).CreateInstance<T>())
        {
            Domain = (T)base.Domain;
        }
    }
}