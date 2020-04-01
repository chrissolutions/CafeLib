using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;

namespace CafeLib.Data.Sources
{
    public class SqlCommandProvider : ISqlCommandProvider
    {
        public Task<bool> DeleteAsync<T>(IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteAsync(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<T>> ExecuteQueryAsync<T>(IConnectionInfo connectionInfo, string sql, object parameters, CancellationToken token) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<SaveResult<TKey>> ExecuteUpsert<TKey>(IConnectionInfo connectionInfo, string sql, object parameters,
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> InsertAsync<T>(IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync<T>(IConnectionInfo connectionInfo, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertAsync<T>(IConnectionInfo connectionInfo, T data, Expression<Func<T, object>>[] expressions,
            CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertAsync<T>(IConnectionInfo connectionInfo, IEnumerable<T> data, Expression<Func<T, object>>[] expressions,
            CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }
    }
}
