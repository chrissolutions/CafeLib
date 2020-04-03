using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Data.Extensions;
using CafeLib.Data.Sources;
using CafeLib.Data.Sources.Extensions;
using CafeLib.Data.SqlGenerator;
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
        private readonly IConnectionOptions _options;

        /// <summary>
        /// Repository constructor.
        /// </summary>
        /// <param name="storage"></param>
        internal Repository(IStorage storage)
        {
            _storage = (StorageBase)storage;
            _tableName = _storage.ConnectionInfo.Domain.TableCache.TableName<T>();
            _propertyCache = _storage.ConnectionInfo.Domain.PropertyCache;
            _options = _storage.ConnectionInfo.Options;
            _modelInfoProvider = new EntityModelInfoProvider(_storage.ConnectionInfo.Domain);
        }

        /// <summary>
        /// Determine whether the entity has any entities.
        /// </summary>
        /// <typeparam name="T">IEntity type</typeparam>
        /// <returns>
        ///     true: if the entity has entries.
        ///     false: if the entity is empty.
        /// </returns>
        public async Task<bool> Any()
        {
            return await Count() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Any<TKey>(TKey key)
        {
            var keyColumnName = _propertyCache.PrimaryKeyName<T>();
            var result = await ExecuteCommand($@"SELECT TOP 1 {keyColumnName} FROM {_tableName} WHERE {keyColumnName} = {CheckSqlKey(key)}");
            return result == 1;
        }

        /// <summary>
        /// Determine whether the entity has any entities.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="predicate">query condition</param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     true: if the entity has entries.
        ///     false: if the entity is empty.
        /// </returns>
        public async Task<bool> Any(Expression<Func<T, bool>> predicate, object? parameters)
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
        public async Task<int> Count(Expression<Func<T, bool>> predicate, object? parameters)
        {
            var query = new List<T>().AsQueryable().Where(predicate);
            var script = QueryTranslator.Translate(query.Expression, _modelInfoProvider, _options.DbObjectFactory);
            var sql = $"select count(*) {script.ToString().Substring(script.ToString().IndexOf("from", StringComparison.Ordinal))}";
            using var connection = _storage.GetConnection();
            return await connection.ExecuteScalarAsync<int>(sql, parameters);
        }

        /// <summary>
        /// Find collection matching the predicate.
        /// </summary>
        /// <param name="predicate">predicate used to filter the query</param>
        /// <param name="parameters">predicate parameters</param>
        /// <returns>collection matching the predicate</returns>
        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, object? parameters)
        {
            var query = new List<T>().AsQueryable().Where(predicate);
            var script = QueryTranslator.Translate(query.Expression, _modelInfoProvider, _options.DbObjectFactory);
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
            using var connection = _storage.GetConnection();
            return await connection.QueryAsync<T>(sql).ConfigureAwait(false);
        }

        /// <summary>
        /// Find first record matching the predicate.
        /// </summary>
        /// <param name="predicate">predicate used to filter the query</param>
        /// <param name="parameters">predicate parameters</param>
        /// <returns></returns>
        public async Task<T> FindOne(Expression<Func<T, bool>> predicate, object? parameters)
        {
            var query = new List<T>().AsQueryable().Where(predicate);
            var script = QueryTranslator.Translate(query.Expression, _modelInfoProvider, _options.DbObjectFactory);
            var sql = $"{script} limit 1";
            var results = await FindBySqlQuery(sql, parameters).ConfigureAwait(false);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> FindByKey<TKey>(TKey key)
        {
            var keyName = _propertyCache.PrimaryKeyName<T>();
            var sql = $"select * from {_tableName} where {keyName} = @Key";
            using var connection = _storage.GetConnection();
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
            var key = _propertyCache.PrimaryKeyName<T>();
            var sql = $"select * from {_tableName} where {key} = @Keys;";
            using var connection = _storage.GetConnection();
            return await connection.QueryAsync<T>(sql, new { Keys = keys }).ConfigureAwait(false);
        }

        /// <summary>
        /// Find entities via sql query.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindBySqlQuery(string sql, object? parameters)
        {
            var result = await ExecuteQuery(sql, parameters);
            return result.Records;
        }

        /// <summary>
        /// Execute sql query.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<QueryResult<T>> ExecuteQuery(string sql, object? parameters)
        {
            return await _storage.ConnectionInfo.ExecuteQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> Add(T entity)
        {
            return await _storage.ConnectionInfo.InsertAsync(entity);
        }

        /// <summary>
        /// Add collection to repository.
        /// </summary>
        /// <param name="entities">collection of entities</param>
        /// <returns></returns>
        public async Task<bool> Add(IEnumerable<T> entities)
        {
            return await _storage.ConnectionInfo.InsertAsync(entities) > 0;
        }

        /// <summary>
        /// Remove entity via its key
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="key">entity key</param>
        /// <returns></returns>
        public async Task<bool> RemoveByKey<TKey>(TKey key)
        {
            var rows = await ExecuteCommand($"DELETE FROM {_tableName} WHERE {_propertyCache.PrimaryKeyName<T>()} = {CheckSqlKey(key)}");
            return rows == 1;
        }

        /// <summary>
        /// Remove a group of entities via their key.
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="keys">collection of keys</param>
        /// <returns></returns>
        public async Task<bool> RemoveByKey<TKey>(IEnumerable<TKey> keys)
        {
            const string openParen = "(";
            const string closeParen = ")";
            const string separator = ", ";

            var builder = new StringBuilder(openParen);
            var keyList = keys.ToArray();
            foreach (var key in keyList)
            {
                builder.Append($"{separator}{CheckSqlKey(key)}");
            }

            builder.Remove(1, separator.Length);
            builder.Append(closeParen);

            var rows = await ExecuteCommand($"DELETE FROM {_tableName} WHERE {_propertyCache.PrimaryKeyName<T>()} IN {builder}");
            return rows == keyList.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Remove(T entity)
        {
            return await _storage.ConnectionInfo.DeleteAsync(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<bool> Remove(IEnumerable<T> entities)
        {
            return await _storage.ConnectionInfo.DeleteAsync(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<int> Remove(Expression<Func<T, bool>> predicate, object? parameters)
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
            return await _storage.ConnectionInfo.UpsertAsync(entities, expressions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Update(T entity)
        {
            return await _storage.ConnectionInfo.UpdateAsync(entity);
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
        public async Task<int> ExecuteCommand(string sql, object? parameters = null)
        {
            return await _storage.ConnectionInfo.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Execute sql command to save an entry.
        /// </summary>
        /// <typeparam name="TKey">entity key type</typeparam>
        /// <param name="sql">sql command text</param>
        /// <param name="parameters">sql command parameters</param>
        /// <returns></returns>
        public async Task<SaveResult<TKey>> ExecuteSave<TKey>(string sql, object? parameters)
        {
            return await _storage.ConnectionInfo.ExecuteSave<TKey>(sql, parameters);
        }

        #region Helpers

        /// <summary>
        /// Check Sql key type for requiring quotation.
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="id"></param>
        /// <returns>return quoted id for certain types</returns>
        private static string CheckSqlKey<TKey>(TKey id)
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

        #endregion
    }
}