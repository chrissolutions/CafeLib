using System.Data.SqlClient;
using CafeLib.Core.Data;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Persistence
{
    /// <summary>
    /// Database storage wrapper class.
    /// </summary>
    public class StorageBase : IStorage
    {
        #region Member Variables

        private const int DefaultConnectionTimeout = 15; // seconds

        private bool _disposed;

        #endregion

        #region Automatic Properties

        public IConnectionInfo ConnectionInfo { get; }

        public int ConnectionTimeout { get; set; }

        public string DatabaseName => ConnectionInfo.ConnectionName;

        public SqlConnection GetConnection() => new SqlConnection(ConnectionInfo.ConnectionString);

        protected internal Domain Domain { get; }

        internal EntityRegistry Repositories { get; set; }

        #endregion

        #region Constructors

        protected StorageBase(IConnectionInfo connectionInfo, Domain domain)
        {
            ConnectionInfo = connectionInfo;
            ConnectionTimeout = connectionInfo.ConnectionTimeout > 0 ? connectionInfo.ConnectionTimeout : DefaultConnectionTimeout;
            Domain = domain;
            Repositories = new EntityRegistry(this);
        }

        #endregion

        #region Methods

        public IRepository<T> CreateRepository<T>() where T : class, IEntity
        {
            var repo = Repositories.Find<T>();
            if (repo != null) return repo;

            repo = new Repository<T>(this);
            Repositories.Add(repo);
            return repo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            return Repositories.Find<T>();
        }

        /// <summary>
        /// Closes and disposes the storage.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            // Dispose the repository registry.
            Repositories.Dispose();

            // Flag storage as disposed.
            _disposed = true;
        }

        #endregion
    }
}