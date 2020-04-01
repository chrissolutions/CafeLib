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
        /// Delete entity from table.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to delete</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if deleted, false if not found</returns>
        Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="domain"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Execute sql query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<QueryResult<T>> ExecuteQueryAsync<T>(IDbConnection connection, string sql, object parameters) where T : class, IEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<SaveResult<TU>> ExecuteUpsert<TU>(IDbConnection connection, string sql, object parameters);

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
        /// <returns></returns>
        Task<int> InsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to be updated</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        Task<bool> UpdateAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Sql update command
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entity to be updated</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        Task<bool> UpdateAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity;

        /// <summary>
        /// Inserts entities into table <typeparamref name="T"/>s (by default).
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="domain">Entity domain</param>
        /// <param name="data">Entities to insert</param>
        /// <param name="expressions"></param>
        /// <param name="token">Cancellation token</param>
        Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, T data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity;

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
    }
}
