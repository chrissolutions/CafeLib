using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Mapping;
using CafeLib.Data.Sources;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Data
{
    public interface IMappedRepository<TModel, TEntity> where TModel : MappedEntity<TModel, TEntity> where TEntity : class, IEntity
    {
        Task<bool> Any();

        Task<bool> Any<TU>(TU id);

        Task<bool> Any(Expression<Func<TModel, bool>> predicate, params object[] parameters);

        Task<int> Count();

        Task<int> Count(Expression<Func<TModel, bool>> predicate, params object[] parameters);

        Task<IEnumerable<TModel>> Find(Expression<Func<TModel, bool>> predicate, params object[] parameters);

        Task<IEnumerable<TModel>> FindAll();

        Task<TModel> FindOne(Expression<Func<TModel, bool>> predicate, params object[] parameters);

        Task<TModel> FindByKey<TKey>(TKey id);

        Task<IEnumerable<TModel>> FindByKey<TKey>(IEnumerable<TKey> keys);

        Task<IEnumerable<TModel>> FindBySqlQuery(string sql, params object[] parameters);

        Task<QueryResult<TModel>> ExecuteQuery(string sql, params object[] parameters);

        Task<TModel> Add(TModel model);

        Task<bool> Add(IEnumerable<TModel> models);

        Task<bool> RemoveByKey<TKey>(TKey key);

        Task<int> RemoveByKey<TKey>(IEnumerable<TKey> keys);

        Task<bool> Remove(TModel model);

        Task<int> Remove(IEnumerable<TModel> models);

        Task<int> Remove(Expression<Func<TModel, bool>> predicate, params object[] parameters);

        Task<TModel> Save(TModel model);

        Task<int> Save(IEnumerable<TModel> models, params Expression<Func<TModel, object>>[] expressions);

        Task<bool> Update(TModel model);

        Task<int> Update(IEnumerable<TModel> models);

        Task<SaveResult<TU>> ExecuteSave<TU>(string sql, params object[] parameters);

        Task<int> ExecuteCommand(string sql, params object[] parameters);
    }
}
