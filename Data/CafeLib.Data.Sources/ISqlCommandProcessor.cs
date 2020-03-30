using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Sources
{
    public interface ISqlCommandProcessor
    {
        /// <summary>
        /// Sql insert command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data"></param>
        /// <returns></returns>
        T Insert<T>(IDbConnection connection, Domain domain, T data) where T : IEntity;

        /// <summary>
        /// Bulk insert entities.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Data source connection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        int Insert<T>(IDbConnection connection, Domain domain, IEnumerable<T> data) where T : IEntity;

        /// <summary>
        /// Sql insert command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">Data source connection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        Task<T> InsertAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Bulk insert entities asynchronously.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Data source connection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="token">Cancellation token</param>
        Task<int> InsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to be updated</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        bool Update<T>(IDbConnection connection, Domain domain, T data) where T : IEntity;

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to be updated</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        bool UpdateAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="expressions"></param>
        int Upsert<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions) where T : IEntity;

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to delete</param>
        /// <returns>true if deleted, false if not found</returns>
        bool Delete<T>(IDbConnection connection, Domain domain, T data) where T : IEntity;

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to delete</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if deleted, false if not found</returns>
        Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity;
    }
}
