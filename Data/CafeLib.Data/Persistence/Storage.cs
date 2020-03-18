using CafeLib.Core.Extensions;
using CafeLib.Data.Dto;

namespace CafeLib.Data.Persistence
{
    /// <summary>
    /// Database storage wrapper class.
    /// </summary>
    public class Storage<T> : StorageBase where T : DtoContext
    {
        /// <summary>
        /// Context.
        /// </summary>
        protected new T Context { get; }

        /// <summary>
        /// Constructs a storage from the connection uri.
        /// </summary>
        /// <param name="connectionUri"></param>
        public Storage(string connectionUri)
            : this(new StorageConnectionInfo(connectionUri))
        {
        }

        /// <summary>
        /// Constructs a storage from the connection uri.
        /// </summary>
        /// <param name="connectionInfo"></param>
        private Storage(IConnectionInfo connectionInfo)
            : base(connectionInfo, typeof(T).CreateInstance<T>())
        {
            Context = (T)base.Context;
        }
    }
}