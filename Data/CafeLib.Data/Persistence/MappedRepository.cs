using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Mapping;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Persistence
{
    public class MappedRepository<TDataModel, TDto> : IMappedRepository<TDataModel, TDto>
        where TDataModel : class, IDataModel where TDto : MappedEntity<TDto>
    {
        private readonly IRepository<TDto> _repository;
        private readonly ExpressionConverter<TDataModel, TDto> _expressionConverter;

        /// <summary>
        /// Repository constructor.
        /// </summary>
        /// <param name="storage"></param>
        internal MappedRepository(IStorage storage)
        {
            _repository = ((StorageBase) storage).Repositories.Find<TDto>();
            _expressionConverter = new ExpressionConverter<TDataModel, TDto>();
        }

        public Task<bool> Any()
        {
            return _repository.Any();
        }

        public Task<bool> Any<TU>(TU id)
        {
            return _repository.Any(id);
        }

        public Task<bool> Any(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TDto, bool>>) _expressionConverter.Visit(predicate);
            return _repository.Any(expr, parameters);
        }

        public Task<int> Count()
        {
            return _repository.Count();
        }

        public Task<int> Count(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TDto, bool>>) _expressionConverter.Visit(predicate);
            return _repository.Count(expr, parameters);
        }

        public async Task<IEnumerable<TDataModel>> Find(Expression<Func<TDataModel, bool>> predicate,
            params object[]? parameters)
        {
            var expr = (Expression<Func<TDto, bool>>) _expressionConverter.Visit(predicate);
            var results = await _repository.Find(expr, parameters);
            return results.Select(x => x.ToModel(Activator.CreateInstance<TDataModel>()));
        }

        public async Task<IEnumerable<TDataModel>> FindAll()
        {
            var results = await _repository.FindAll();
            return results.Select(x => x.ToModel(Activator.CreateInstance<TDataModel>()));
        }

        public async Task<TDataModel> FindOne(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TDto, bool>>) _expressionConverter.Visit(predicate);
            var dto = await _repository.FindOne(expr, parameters);
            return dto.ToModel(Activator.CreateInstance<TDataModel>());
        }

        public async Task<TDataModel> FindByKey<TKey>(TKey key)
        {
            var dto = await _repository.FindByKey(key);
            return dto?.ToModel(Activator.CreateInstance<TDataModel>()) ?? default!;
        }

        public async Task<IEnumerable<TDataModel>> FindByKey<TKey>(IEnumerable<TKey> keys)
        {
            var results = await _repository.FindByKey(keys);
            return results.Select(x => x.ToModel(Activator.CreateInstance<TDataModel>()));
        }

        public async Task<IEnumerable<TDataModel>> FindBySqlQuery(string sql, params object[]? parameters)
        {
            var results = await _repository.FindBySqlQuery(sql, parameters);
            return results.Select(x => x.ToModel(Activator.CreateInstance<TDataModel>()));
        }

        public async Task<QueryResult<TDataModel>> ExecuteQuery(string sql, params object[]? parameters)
        {
            var result = await _repository.ExecuteQuery(sql, parameters);
            return new QueryResult<TDataModel>
            {
                Records = result.Records.Select(x => x.ToModel(Activator.CreateInstance<TDataModel>())),
                TotalCount = result.TotalCount
            };
        }

        public async Task<TDataModel> Add(TDataModel entity)
        {
            var dto = (TDto) Activator.CreateInstance<TDto>().FromModel(entity);
            var result = await _repository.Add(dto);
            return result.ToModel(entity);
        }

        public async Task<bool> Add(IEnumerable<TDataModel> entities)
        {
            var results = entities.Select(Activator.CreateInstance<TDto>().FromModel).Cast<TDto>();
            return await _repository.Add(results);
        }

        public Task<bool> RemoveById<TU>(TU id)
        {
            return _repository.RemoveById(id);
        }

        public Task<bool> RemoveById<TU>(IEnumerable<TU> ids)
        {
            return _repository.RemoveById(ids);
        }

        public async Task<bool> Remove(TDataModel entity)
        {
            var dto = (TDto) Activator.CreateInstance<TDto>().FromModel(entity);
            return await _repository.Remove(dto);
        }

        public async Task<bool> Remove(IEnumerable<TDataModel> entities)
        {
            var results = entities.Select(Activator.CreateInstance<TDto>().FromModel).Cast<TDto>();
            return await _repository.Remove(results);
        }

        public async Task<int> Remove(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters)
        {
            var expr = (Expression<Func<TDto, bool>>) _expressionConverter.Visit(predicate);
            return await _repository.Remove(expr, parameters);
        }

        public async Task<TDataModel> Save(TDataModel entity)
        {
            var dto = (TDto) Activator.CreateInstance<TDto>().FromModel(entity);
            var result = await _repository.Save(dto);
            return result.ToModel(entity);
        }

        public async Task<int> Save(IEnumerable<TDataModel> entities,
            params Expression<Func<TDataModel, object>>[]? expressions)
        {
            var results = entities.Select(Activator.CreateInstance<TDto>().FromModel).Cast<TDto>();
            var lambdas = expressions.Select(_expressionConverter.Visit).Cast<Expression<Func<TDto, object>>>()
                .ToArray();
            return await _repository.Save(results, lambdas);
        }

        public async Task<bool> Update(TDataModel entity)
        {
            var dto = (TDto) Activator.CreateInstance<TDto>().FromModel(entity);
            return await _repository.Update(dto);
        }

        public async Task<bool> Update(IEnumerable<TDataModel> entities)
        {
            var results = entities.Select(Activator.CreateInstance<TDto>().FromModel).Cast<TDto>();
            return await _repository.Update(results);
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