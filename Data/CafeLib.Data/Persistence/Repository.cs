using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Data.Extensions;
using CafeLib.Data.Sources;
using CafeLib.Data.SqlGenerator;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;
using CafeLib.Data.SqlGenerator.Models;
using Dapper;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Persistence
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly StorageBase _storage;
        private readonly string _tableName;
        private readonly PropertyCache _propertyCache;
        private readonly IModelInfoProvider _modelInfoProvider;
        private readonly IDbObjectFactory _dbObjectFactory;

        /// <summary>
        /// Repository constructor.
        /// </summary>
        /// <param name="storage"></param>
        internal Repository(IStorage storage)
        {
            _storage = (StorageBase)storage;
            _tableName = _storage.Domain.TableCache.TableName<T>();
            _propertyCache = _storage.Domain.PropertyCache;
            _modelInfoProvider = new EntityModelInfoProvider(_storage.Domain);
            _dbObjectFactory = new SqlObjectFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Any()
        {
            return await Count() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Any<TU>(TU id)
        {
            var keyColumnName = _propertyCache.PrimaryKeyName<T>();
            var result = await ExecuteCommand($@"SELECT TOP 1 {keyColumnName} FROM {_tableName} WHERE {keyColumnName} = {CheckSqlId(id)}");
            return result == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<bool> Any(Expression<Func<T, bool>> predicate, params object[]? parameters)
        {
            return await Count(predicate, parameters) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<int> Count()
        {
            return _storage.GetConnection().ExecuteScalarAsync<int>($"select count(*) from {_tableName}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> Count(Expression<Func<T, bool>> predicate, params object[]? parameters)
        {
            var query = new List<T>().AsQueryable().Where(predicate);
            var script = QueryTranslator.Translate(query.Expression, _modelInfoProvider, _dbObjectFactory);
            var sql = $"select count(*) {script.ToString().Substring(script.ToString().IndexOf("from", StringComparison.Ordinal))}";
            await using var connection = _storage.GetConnection();
            return connection.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Find collection matching the predicate.
        /// </summary>
        /// <param name="predicate">predicate used to filter the query</param>
        /// <param name="parameters">predicate parameters</param>
        /// <returns>collection matching the predicate</returns>
        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, params object[]? parameters)
        {
            var query = new List<T>().AsQueryable().Where(predicate);
            var script = QueryTranslator.Translate(query.Expression, _modelInfoProvider, _dbObjectFactory);
            var sql = script.ToString();
            var results = await ExecuteQuery(sql, parameters).ConfigureAwait(false);
            return results.Records;
        }

        /// <summary>
        /// Gets all records from the table.
        /// </summary>
        /// <returns>collection of all table records</returns>
        public async Task<IEnumerable<T>> FindAll()
        {
            var sql = $"select * from {_tableName}";
            await using var connection = _storage.GetConnection();
            return await connection.QueryAsync<T>(sql).ConfigureAwait(false);
        }

        /// <summary>
        /// Find first record matching the predicate.
        /// </summary>
        /// <param name="predicate">predicate used to filter the query</param>
        /// <param name="parameters">predicate parameters</param>
        /// <returns></returns>
        public async Task<T> FindOne(Expression<Func<T, bool>> predicate, params object[]? parameters)
        {
            var query = new List<T>().AsQueryable().Where(predicate);
            var script = QueryTranslator.Translate(query.Expression, _modelInfoProvider, _dbObjectFactory);
            var sql = $"{script} limit 1";
            var results = await ExecuteQuery(sql, parameters).ConfigureAwait(false);
            return results.Records.FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> FindByKey<TKey>(TKey key)
        {
            var keyName = _storage.Domain.PropertyCache.PrimaryKeyName<T>();
            var sql = $"select * from {_tableName} where {keyName} = @Key";
            await using var connection = _storage.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, new {Key = key}).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindByKey<TKey>(IEnumerable<TKey> keys)
        {
            var key = _storage.Domain.PropertyCache.PrimaryKeyName<T>();
            var sql = $"select * from {_tableName} where {key} = @Keys;";
            await using var connection = _storage.GetConnection();
            return await connection.QueryAsync<T>(sql, new { Keys = keys }).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindBySqlQuery(string sql, params object[]? parameters)
        {
            await using var connection = _storage.GetConnection();
            return await connection.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<QueryResult<T>> ExecuteQuery(string sql, params object[]? parameters)
        {
            await using var connection = _storage.GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters?.Any() ?? false)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            await using var reader = await command.ExecuteReaderAsync();
            var totalCount = -1;
            var results = new List<T>();

            var model = Activator.CreateInstance<T>();
            while (reader.Read())
            {
                foreach (var prop in model.GetType().GetProperties())
                {
                    var attr = prop.GetCustomAttribute(typeof(ColumnAttribute));
                    var name = attr != null ? ((ColumnAttribute)attr).Name : prop.Name;
                    var val = reader[name];
                    prop.SetValue(model, val == DBNull.Value ? null : val);
                }
                results.Add(model);
                model = Activator.CreateInstance<T>();
            }

            if (reader.NextResult())
            {
                reader.Read();
                totalCount = reader.GetInt32(0);
            }
            return new QueryResult<T> { Records = results.ToArray(), TotalCount = totalCount };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> Add(T entity)
        {
            await using var connection = _storage.GetConnection();
            return connection.Insert(_storage.Domain, entity);
        }

        /// <summary>
        /// Add collection to repository.
        /// </summary>
        /// <param name="entities">collection of entities</param>
        /// <returns></returns>
        public async Task<bool> Add(IEnumerable<T> entities)
        {
            await using var connection = _storage.GetConnection();
            return connection.BulkInsert(_storage.Domain, entities) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> RemoveById<TU>(TU id)
        {
            var rows = await ExecuteCommand($"DELETE FROM {_tableName} WHERE {_propertyCache.PrimaryKeyName<T>()} = {CheckSqlId(id)}");
            return rows == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="idCollection"></param>
        /// <returns></returns>
        public async Task<bool> RemoveById<TU>(IEnumerable<TU> idCollection)
        {
            const string openParen = "(";
            const string closeParen = ")";
            const string separator = ", ";

            var builder = new StringBuilder(openParen);
            var ids = idCollection.ToArray();
            foreach (var id in ids)
            {
                builder.Append($"{separator}{CheckSqlId(id)}");
            }

            builder.Remove(1, separator.Length);
            builder.Append(closeParen);

            var rows = await ExecuteCommand($"DELETE FROM {_tableName} WHERE {_propertyCache.PrimaryKeyName<T>()} IN {builder}");
            return rows == ids.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Remove(T entity)
        {
            await using var connection = _storage.GetConnection();
            return connection.Delete(_storage.Domain, entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<bool> Remove(IEnumerable<T> entities)
        {
            var result = true;
            await entities.ForEachAsync(async x => result &= await Remove(x));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> Remove(Expression<Func<T, bool>> predicate, params object[]? parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> Save(T entity)
        {
            if (entity.IsKeyGenerated() && entity.KeyValue() == default(T) || !await Any(entity.KeyValue()))
            {
                return await Add(entity);
            }

            await Update(entity);
            return entity;
        }

        /// <summary>
        /// Bulk save.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public async Task<int> Save(IEnumerable<T> entities, params Expression<Func<T, object>>[]? expressions)
        {
            await using var connection = _storage.GetConnection();
            var propertyExpressions = expressions != null && expressions.Any() ? new PropertyExpressionList<T>(_storage.Domain, expressions) : null;
            return connection.BulkUpsert(_storage.Domain, entities, propertyExpressions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Update(T entity)
        {
            await using var connection = _storage.GetConnection();
            return connection.Update(_storage.Domain, entity);
        }

        /// <summary>
        /// Bulk Update
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<bool> Update(IEnumerable<T> entities)
        {
            var result = true;
            await entities.ForEachAsync(async x => result &= await Update(x));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteCommand(string sql, params object[]? parameters)
        {
            await using var connection = _storage.GetConnection();
            return await connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<SaveResult<TU>> ExecuteSave<TU>(string sql, params object[]? parameters)
        {
            await using var connection = _storage.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters?.Any() ?? false)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            var inserted = false;
            var id = (TU) DefaultSqlId<TU>();

            var result = command.ExecuteScalar();
            if (result != null)
            {
                id = (TU)result;
                inserted = true;
            }

            return new SaveResult<TU>(id, inserted);
        }

        #region Helpers

        /// <summary>
        /// Check Sql Id type for requiring quotation.
        /// </summary>
        /// <typeparam name="TU">identifier type</typeparam>
        /// <param name="id"></param>
        /// <returns>return quoted id for certain types</returns>
        private static string CheckSqlId<TU>(TU id)
        {
            switch (id?.GetType().Name)
            {
                case "String":
                case "Guid":
                    return $"'{id}'";

                case null:
                    return @"NULL";

                default:
                    return $"{id}";
            }
        }

        /// <summary>
        /// Check Sql Id type for requiring quotation.
        /// </summary>
        /// <typeparam name="TU">identifier type</typeparam>
        /// <returns>return quoted id for certain types</returns>
        private static object DefaultSqlId<TU>()
        {
            switch (typeof(TU).Name)
            {
                case "String":
                    return string.Empty;

                case "Guid":
                    return Guid.Empty;

                default:
                    return 0;
            }
        }

        #endregion
    }
}