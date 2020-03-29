using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;

namespace CafeLib.Data.Options.SqlServer
{
    public class SqlServerCommandProcessor : ISqlCommandProcessor
    {
        public T Insert<T>(IDbConnection connection, Domain domain, T data) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public int Insert<T>(IDbConnection connection, Domain domain, IEnumerable<T> data) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<T> InsertAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(IDbConnection connection, Domain domain, T data) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public bool UpdateAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public int Upsert<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertAsync<T>(IDbConnection connection, Domain domain, IEnumerable<T> data, Expression<Func<T, object>>[] expressions,
            CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(IDbConnection connection, Domain domain, T data) where T : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync<T>(IDbConnection connection, Domain domain, T data, CancellationToken token = default) where T : IEntity
        {
            throw new NotImplementedException();
        }
    }
}
