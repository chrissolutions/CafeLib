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
    public class MappedRepository<TModel, TEntity> : IMappedRepository<TModel, TEntity> where TModel : MappedClass<TModel, TEntity>, new() where TEntity : class, IEntity
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

        public Task<bool> Any(Expression<Func<TModel, bool>> predicate, params object[] parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            return _repository.Any(expr, parameters);
        }

        public Task<int> Count()
        {
            return _repository.Count();
        }

        public Task<int> Count(Expression<Func<TModel, bool>> predicate, params object[] parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            return _repository.Count(expr, parameters);
        }

        public async Task<IEnumerable<TModel>> Find(Expression<Func<TModel, bool>> predicate, params object[] parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            var results = await _repository.Find(expr, parameters);
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<IEnumerable<TModel>> FindAll()
        {
            var results = await _repository.FindAll();
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<TModel> FindOne(Expression<Func<TModel, bool>> predicate, params object[] parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            var entity = await _repository.FindOne(expr, parameters);
            return entity != null ? new TModel().Populate(entity) : null;
        }

        public async Task<TModel> FindByKey<TKey>(TKey key)
        {
            var entity = await _repository.FindByKey(key);
            return entity != null ? new TModel().Populate(entity) : null;
        }

        public async Task<IEnumerable<TModel>> FindByKey<TKey>(IEnumerable<TKey> keys)
        {
            var results = await _repository.FindByKey(keys);
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<IEnumerable<TModel>> FindBySqlQuery(string sql, params object[] parameters)
        {
            var results = await _repository.FindBySqlQuery(sql, parameters);
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<QueryResult<TModel>> ExecuteQuery(string sql, params object[] parameters)
        {
            var result = await _repository.ExecuteQuery(sql, parameters);
            return new QueryResult<TModel>
            {
                Records = result.Records.Select(x => new TModel().Populate(x)),
                TotalCount = result.TotalCount
            };
        }

        public async Task<TModel> Add(TModel model)
        {
            var entity = model.ToEntity();
            var result = await _repository.Add(entity);
            return model.Populate(result);
        }

        public async Task<bool> Add(IEnumerable<TModel> models)
        {
            var entities = models.Select(x => x.ToEntity());
            return await _repository.Add(entities);
        }

        public Task<bool> RemoveByKey<TKey>(TKey key)
        {
            return _repository.RemoveByKey(key);
        }

        public Task<int> RemoveByKey<TKey>(IEnumerable<TKey> keys)
        {
            return _repository.RemoveByKey(keys);
        }

        public async Task<bool> Remove(TModel model)
        {
            var entity = model.ToEntity();
            return await _repository.Remove(entity);
        }

        public async Task<int> Remove(IEnumerable<TModel> models)
        {
            var entities = models.Select(x => x.ToEntity());
            return await _repository.Remove(entities);
        }

        public async Task<int> Remove(Expression<Func<TModel, bool>> predicate, params object[] parameters)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Visit(predicate);
            return await _repository.Remove(expr, parameters);
        }

        public async Task<TModel> Save(TModel model)
        {
            var entity = model.ToEntity();
            var result = await _repository.Save(entity);
            return model.Populate(result);
        }

        public async Task<int> Save(IEnumerable<TModel> models, params Expression<Func<TModel, object>>[] expressions)
        {
            var entities = models.Select(x => x.ToEntity());
            var lambdas = expressions.Select(_expressionConverter.Visit).Cast<Expression<Func<TEntity, object>>>().ToArray();
            return await _repository.Save(entities, lambdas);
        }

        public async Task<bool> Update(TModel model)
        {
            var entity = model.ToEntity();
            return await _repository.Update(entity);
        }

        public async Task<int> Update(IEnumerable<TModel> models)
        {
            var entities = models.Select(x => x.ToEntity());
            return await _repository.Update(entities);
        }

        public async Task<SaveResult<TU>> ExecuteSave<TU>(string sql, params object[] parameters)
        {
            return await _repository.ExecuteSave<TU>(sql, parameters);
        }

        public Task<int> ExecuteCommand(string sql, params object[] parameters)
        {
            return _repository.ExecuteCommand(sql, parameters);
        }
    }
}