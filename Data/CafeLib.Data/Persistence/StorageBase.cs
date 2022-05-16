using System;
using System.Data;
using CafeLib.Core.Data;
using CafeLib.Data.Sources;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Persistence
{
    /// <summary>
    /// Database storage wrapper class.
    /// </summary>
    public class StorageBase : IStorage
    {
        #region Member Variables

        private bool _disposed;

        #endregion

        #region Automatic Properties

        public IConnectionInfo ConnectionInfo { get; }

        public IDbConnection GetConnection() => ConnectionInfo.Options.GetConnection(ConnectionInfo.ConnectionString);

        internal EntityRegistry Repositories { get; set; }

        #endregion

        #region Constructors

        protected StorageBase(IConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;
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
        /// Obtain the responsitory of the IEntity.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>the entity's respository</returns>
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
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose storage.
        /// </summary>
        /// <param name="disposing">disposing flag</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            Repositories.Dispose();
            _disposed = true;
        }

        #endregion
    }
}