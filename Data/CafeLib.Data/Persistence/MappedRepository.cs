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
    public class MappedRepository<TModel, TEntity> : IMappedRepository<TModel, TEntity> where TModel : class, IMappedEntity<TModel, TEntity>, new() where TEntity : class, IEntity
    {
        private readonly ExpressionConverter<TModel, TEntity> _expressionConverter;
        private readonly Lazy<IRepository<TEntity>> _repository;


        /// <summary>
        /// Repository constructor.
        /// </summary>
        /// <param name="storage"></param>
        internal MappedRepository(IStorage storage)
        {
            _expressionConverter = new ExpressionConverter<TModel, TEntity>();
            _repository = new Lazy<IRepository<TEntity>>(() => ((StorageBase) storage).Repositories.Find<TEntity>());
        }

        public Task<bool> Any()
        {
            return _repository.Value.Any();
        }

        public Task<bool> Any<TU>(TU id)
        {
            return _repository.Value.Any(id);
        }

        public Task<bool> Any(Expression<Func<TModel, bool>> predicate)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Convert(predicate);
            return _repository.Value.Any(expr);
        }

        public Task<int> Count()
        {
            return _repository.Value.Count();
        }

        public Task<int> Count(Expression<Func<TModel, bool>> predicate)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Convert(predicate);
            return _repository.Value.Count(expr);
        }

        public async Task<IEnumerable<TModel>> Find(Expression<Func<TModel, bool>> predicate)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Convert(predicate);
            var results = await _repository.Value.Find(expr);
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<IEnumerable<TModel>> FindAll()
        {
            var results = await _repository.Value.FindAll();
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<TModel> FindOne(Expression<Func<TModel, bool>> predicate)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Convert(predicate);
            var entity = await _repository.Value.FindOne(expr);
            return entity != null ? new TModel().Populate(entity) : null;
        }

        public async Task<TModel> FindByKey<TKey>(TKey key)
        {
            var entity = await _repository.Value.FindByKey(key);
            return entity != null ? new TModel().Populate(entity) : null;
        }

        public async Task<IEnumerable<TModel>> FindByKey<TKey>(IEnumerable<TKey> keys)
        {
            var results = await _repository.Value.FindByKey(keys);
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<IEnumerable<TModel>> FindBySqlQuery(string sql, object parameters)
        {
            var results = await _repository.Value.FindBySqlQuery(sql, parameters);
            return results.Select(x => new TModel().Populate(x));
        }

        public async Task<QueryResult<TModel>> ExecuteQuery(string sql, object parameters)
        {
            var result = await _repository.Value.ExecuteQuery(sql, parameters);
            return new QueryResult<TModel>
            {
                Records = result.Records.Select(x => new TModel().Populate(x)),
                TotalCount = result.TotalCount
            };
        }

        public async Task<TModel> Add(TModel model)
        {
            var entity = model.ToEntity();
            var result = await _repository.Value.Add(entity);
            return model.Populate(result);
        }

        public async Task<bool> Add(IEnumerable<TModel> models)
        {
            var entities = models.Select(x => x.ToEntity());
            return await _repository.Value.Add(entities);
        }

        public Task<bool> RemoveByKey<TKey>(TKey key)
        {
            return _repository.Value.RemoveByKey(key);
        }

        public Task<int> RemoveByKey<TKey>(IEnumerable<TKey> keys)
        {
            return _repository.Value.RemoveByKey(keys);
        }

        public async Task<bool> Remove(TModel model)
        {
            var entity = model.ToEntity();
            return await _repository.Value.Remove(entity);
        }

        public async Task<int> Remove(IEnumerable<TModel> models)
        {
            var entities = models.Select(x => x.ToEntity());
            return await _repository.Value.Remove(entities);
        }

        public async Task<int> Remove(Expression<Func<TModel, bool>> predicate)
        {
            var expr = (Expression<Func<TEntity, bool>>) _expressionConverter.Convert(predicate);
            return await _repository.Value.Remove(expr);
        }

        public async Task<TModel> Save(TModel model)
        {
            var entity = model.ToEntity();
            var result = await _repository.Value.Save(entity);
            return model.Populate(result);
        }

        public async Task<int> Save(IEnumerable<TModel> models, params Expression<Func<TModel, object>>[] expressions)
        {
            var entities = models.Select(x => x.ToEntity());
            var lambdas = expressions.Select(_expressionConverter.Visit).Cast<Expression<Func<TEntity, object>>>().ToArray();
            return await _repository.Value.Save(entities, lambdas);
        }

        public async Task<bool> Update(TModel model)
        {
            var entity = model.ToEntity();
            return await _repository.Value.Update(entity);
        }

        public async Task<int> Update(IEnumerable<TModel> models)
        {
            var entities = models.Select(x => x.ToEntity());
            return await _repository.Value.Update(entities);
        }

        public async Task<SaveResult<TU>> ExecuteSave<TU>(string sql, object parameters)
        {
            return await _repository.Value.ExecuteSave<TU>(sql, parameters);
        }

        public Task<int> ExecuteCommand(string sql, object parameters)
        {
            return _repository.Value.ExecuteCommand(sql, parameters);
        }
    }
}