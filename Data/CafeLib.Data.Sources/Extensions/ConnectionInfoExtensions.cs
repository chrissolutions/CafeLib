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
        public static T GetConnection<T>(this IConnectionInfo connectionInfo) where T : IDbConnection
        {
            return (T)connectionInfo.Options.GetConnection(connectionInfo.ConnectionString);
        }

        public static async Task<bool> DeleteAsync<T>(this IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.DeleteAsync(connectionInfo, data, token);
        }

        public static async Task<bool> DeleteAsync<T>(this IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.DeleteAsync(connectionInfo, data, token);
        }

        public static async Task<int> ExecuteAsync(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            return await connectionInfo.Options.CommandProcessor.ExecuteAsync(connectionInfo, sql, parameters, token);
        }

        public static async Task<QueryResult<T>> ExecuteQueryAsync<T>(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.ExecuteQueryAsync<T>(connectionInfo, sql, parameters, token);
        }

        public static async Task<SaveResult<TKey>> ExecuteSave<TKey>(this IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            return await connectionInfo.Options.CommandProcessor.ExecuteSave<TKey>(connectionInfo, sql, parameters, token);
        }

        public static async Task<T> InsertAsync<T>(this IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.InsertAsync(connectionInfo, data, token);
        }

        public static async Task<int> InsertAsync<T>(this IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.InsertAsync(connectionInfo, data, token);
        }

        public static async Task<bool> UpdateAsync<T>(this IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.UpdateAsync(connectionInfo, data, token);
        }

        public static async Task<bool> UpdateAsync<T>(this IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.UpdateAsync(connectionInfo, data, token);
        }

        public static async Task<T> UpsertAsync<T>(this IConnectionInfo connectionInfo, T data, Expression<Func<T, object>>[] expressions = null, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.UpsertAsync(connectionInfo, data, expressions, token);
        }

        public static async Task<int> UpsertAsync<T>(this IConnectionInfo connectionInfo, IEnumerable<T> data, Expression<Func<T, object>>[] expressions = null, CancellationToken token = default) where T : IEntity
        {
            return await connectionInfo.Options.CommandProcessor.UpsertAsync(connectionInfo, data, expressions, token);
        }
    }
}
