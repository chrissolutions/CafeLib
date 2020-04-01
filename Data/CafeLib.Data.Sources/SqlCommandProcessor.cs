using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;

namespace CafeLib.Data.Sources
{
    public class SqlCommandProcessor : ISqlCommandProcessor
    {
        public Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<T>> ExecuteQueryAsync<T>(IDbConnection connection, string sql, object parameters) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<SaveResult<TU>> ExecuteUpsert<TU>(IDbConnection connection, string sql, object parameters)
        {
            throw new NotImplementedException();
        }

        public Task<T> InsertAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, T data, Expression<Func<T, object>>[] expressions,
            CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }
    }
}
