using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Mapping;
using CafeLib.Data.Sources;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Persistence
{
    public class MappedRepository<TModel, TEntity> : IMappedRepository<TModel, TEntity> where TModel : MappedClass<TModel, TEntity> where TEntity : class, IEntity
    {
        private readonly IRepository<TEntity> _repository;
        private readonly ExpressionConverter<TModel, TEntity> _expressionConverter;

        /// <summary>
        /// Repository constructor.
        /// </summary>
        /// <param name="storage"></param>
        internal MappedRepository(IStorage storage)
        {
            _repository = ((StorageBase) storage).Repositories.Find<TEntity>();
            _expressionConverter = new ExpressionConverter<TModel, TEntity>();
        }

        public Task<bool> Any()
        {
            return _repository.Any();
        }

        public Task<bool> Any<TU>(TU id)
        {
            return _repository.Any(id);
        }

        public Task<bool> Any(Expression<Func<TModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            return _repository.Any(expr, parameters);
        }

        public Task<int> Count()
        {
            return _repository.Count();
        }

        public Task<int> Count(Expression<Func<TModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            return _repository.Count(expr, parameters);
        }

        public async Task<IEnumerable<TModel>> Find(Expression<Func<TModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            var results = await _repository.Find(expr, parameters);
            return default;
            //return results.Select(x => x.ToModel(Activator.CreateInstance<TModel>()));
        }

        public async Task<IEnumerable<TModel>> FindAll()
        {
            var results = await _repository.FindAll();
            return default;
            //return results.Select(x => x.ToModel(Activator.CreateInstance<TModel>()));
        }

        public async Task<TModel> FindOne(Expression<Func<TModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            var dto = await _repository.FindOne(expr, parameters);
            return default;
            //return dto.ToModel(Activator.CreateInstance<TModel>());
        }

        public async Task<TModel> FindByKey<TKey>(TKey key)
        {
            var dto = await _repository.FindByKey(key);
            return default;
            //return dto?.ToModel(Activator.CreateInstance<TModel>()) ?? default!;
        }

        public async Task<IEnumerable<TModel>> FindByKey<TKey>(IEnumerable<TKey> keys)
        {
            var results = await _repository.FindByKey(keys);
            return default;
            //return results.Select(x => x.ToModel(Activator.CreateInstance<TModel>()));
        }

        public async Task<IEnumerable<TModel>> FindBySqlQuery(string sql, params object[]? parameters)
        {
            var results = await _repository.FindBySqlQuery(sql, parameters);
            return default;
            //return results.Select(x => x.ToModel(Activator.CreateInstance<TModel>()));
        }

        public async Task<QueryResult<TModel>> ExecuteQuery(string sql, params object[]? parameters)
        {
            var result = await _repository.ExecuteQuery(sql, parameters);
            return new QueryResult<TModel>
            {
                //Records = result.Records.Select(x => x.ToModel(Activator.CreateInstance<TModel>())),
                TotalCount = result.TotalCount
            };
        }

        public async Task<TModel> Add(TModel entity)
        {
            //var dto = (TEntity) Activator.CreateInstance<TEntity>().FromModel(entity);
            //var result = await _repository.Add(dto);
            //return result.ToModel(entity);
            return default;
        }

        public async Task<bool> Add(IEnumerable<TModel> entities)
        {
            //var results = entities.Select(Activator.CreateInstance<TEntity>().FromModel).Cast<TEntity>();
            //return await _repository.Add(results);
            return default;
        }

        public Task<bool> RemoveByKey<TKey>(TKey key)
        {
            return _repository.RemoveByKey(key);
        }

        public Task<int> RemoveByKey<TKey>(IEnumerable<TKey> keys)
        {
            return _repository.RemoveByKey(keys);
        }

        public async Task<bool> Remove(TModel entity)
        {
            //var dto = (TEntity) Activator.CreateInstance<TEntity>().FromModel(entity);
            //return await _repository.Remove(dto);
            return default;
        }

        public async Task<int> Remove(IEnumerable<TModel> entities)
        {
            //var results = entities.Select(Activator.CreateInstance<TEntity>().FromModel).Cast<TEntity>();
            //return await _repository.Remove(results);
            return default;
        }

        public async Task<int> Remove(Expression<Func<TModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            return await _repository.Remove(expr, parameters);
        }

        public async Task<TModel> Save(TModel entity)
        {
            //var dto = (TEntity) Activator.CreateInstance<TEntity>().FromModel(entity);
            //var result = await _repository.Save(dto);
            //return result.ToModel(entity);
            return default;
        }

        public async Task<int> Save(IEnumerable<TModel> entities,
            params Expression<Func<TModel, object>>[]? expressions)
        {
            //var results = entities.Select(Activator.CreateInstance<TEntity>().FromModel).Cast<TEntity>();
            //var lambdas = expressions.Select(_expressionConverter.Visit).Cast<Expression<Func<TEntity, object>>>()
            //    .ToArray();
            //return await _repository.Save(results, lambdas);
            return default;
        }

        public async Task<bool> Update(TModel entity)
        {
            //var dto = (TEntity) Activator.CreateInstance<TEntity>().FromModel(entity);
            //return await _repository.Update(dto);
            return default;
        }

        public async Task<int> Update(IEnumerable<TModel> entities)
        {
            //var results = entities.Select(Activator.CreateInstance<TEntity>().FromModel).Cast<TEntity>();
            //return await _repository.Update(results);
            return default;
        }

        public async Task<SaveResult<TU>> ExecuteSave<TU>(string sql, params object[]? parameters)
        {
            return await _repository.ExecuteSave<TU>(sql, parameters);
        }

        public Task<int> ExecuteCommand(string sql, params object[] parameters)
        {
            return _repository.ExecuteCommand(sql, parameters);
        }
    }
}