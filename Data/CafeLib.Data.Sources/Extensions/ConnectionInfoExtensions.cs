using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;

namespace CafeLib.Data.Sources.Extensions
{
    public static class ConnectionInfoExtensions
    {
        public static TConnection GetConnection<TConnection>(this IConnectionInfo connectionInfo) where TConnection : IDbConnection => 
            (TConnection)connectionInfo.Options.GetConnection(connectionInfo.ConnectionString);

        public static async Task<bool> DeleteAsync<TEntity>(this IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.DeleteAsync(connectionInfo, data, token);

        public static async Task<int> DeleteAsync<TEntity>(this IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.DeleteAsync(connectionInfo, data, token);

        public static async Task<int> DeleteAsync<TEntity>(this IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.DeleteAsync(connectionInfo, predicate, token);

        public static async Task<bool> DeleteByKeyAsync<TEntity, TKey>(this IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.DeleteByKeyAsync<TEntity, TKey>(connectionInfo, key, token);

        public static async Task<int> DeleteByKeyAsync<TEntity, TKey>(this IConnectionInfo connectionInfo, IEnumerable<TKey> keys, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.DeleteByKeyAsync<TEntity, TKey>(connectionInfo, keys, token);

        public static async Task<int> ExecuteAsync(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) =>
            await connectionInfo.Options.CommandProvider.ExecuteAsync(connectionInfo, sql, parameters, token);

        public static async Task<SaveResult<TKey>> ExecuteSaveAsync<TKey>(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) =>
            await connectionInfo.Options.CommandProvider.ExecuteSaveAsync<TKey>(connectionInfo, sql, parameters, token);

        public static async Task<object> ExecuteScalarAsync(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) =>
            await connectionInfo.Options.CommandProvider.ExecuteScalarAsync(connectionInfo, sql, parameters, token);

        public static async Task<TEntity> InsertAsync<TEntity>(this IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.InsertAsync(connectionInfo, data, token);

        public static async Task<int> InsertAsync<TEntity>(this IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.InsertAsync(connectionInfo, data, token);

        public static async Task<QueryResult<TEntity>> QueryAsync<TEntity>(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryAsync<TEntity>(connectionInfo, sql, parameters, token);

        public static async Task<QueryResult<TEntity>> QueryAsync<TEntity>(this IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryAsync(connectionInfo, predicate, token);

        public static async Task<QueryResult<TEntity>> QueryAllAsync<TEntity>(this IConnectionInfo connectionInfo, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryAllAsync<TEntity>(connectionInfo, token).ConfigureAwait(false);

        public static async Task<TEntity> QueryByKeyAsync<TEntity, TKey>(this IConnectionInfo connectionInfo, TKey key, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryByKeyAsync<TEntity, TKey>(connectionInfo, key, token);

        public static async Task<IEnumerable<TEntity>> QueryByKeyAsync<TEntity, TKey>(this IConnectionInfo connectionInfo, IEnumerable<TKey> keys, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryByKeyAsync<TEntity, TKey>(connectionInfo, keys, token);

        public static async Task<int> QueryCountAsync<TEntity>(this IConnectionInfo connectionInfo, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryCountAsync<TEntity>(connectionInfo, token);

        public static async Task<int> QueryCountAsync<TEntity>(this IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryCountAsync(connectionInfo, predicate, token);

        public static async Task<TEntity> QueryOneAsync<TEntity>(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryOneAsync<TEntity>(connectionInfo, sql, parameters, token);

        public static async Task<TEntity> QueryOneAsync<TEntity>(this IConnectionInfo connectionInfo, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.QueryOneAsync(connectionInfo, predicate, token);

        public static async Task<bool> UpdateAsync<TEntity>(this IConnectionInfo connectionInfo, TEntity data, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.UpdateAsync(connectionInfo, data, token);

        public static async Task<bool> UpdateAsync<TEntity>(this IConnectionInfo connectionInfo, IEnumerable<TEntity> data, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.UpdateAsync(connectionInfo, data, token);

        public static async Task<TEntity> UpsertAsync<TEntity>(this IConnectionInfo connectionInfo, TEntity data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.UpsertAsync(connectionInfo, data, expressions, token);

        public static async Task<int> UpsertAsync<TEntity>(this IConnectionInfo connectionInfo, IEnumerable<TEntity> data, Expression<Func<TEntity, object>>[] expressions = null, CancellationToken token = default) where TEntity : class, IEntity =>
            await connectionInfo.Options.CommandProvider.UpsertAsync(connectionInfo, data, expressions, token);
    }
}
