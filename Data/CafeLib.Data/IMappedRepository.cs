using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CafeLib.Core.Data;
using CafeLib.Data.Mapping;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data
{
    public interface IMappedRepository<TDataModel, TDto> where TDataModel : class, IDataModel where TDto : MappedEntity<TDto>
    {
        Task<bool> Any();

        Task<bool> Any<TU>(TU id);

        Task<bool> Any(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters);

        Task<int> Count();

        Task<int> Count(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters);

        Task<IEnumerable<TDataModel>> Find(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters);

        Task<IEnumerable<TDataModel>> FindAll();

        Task<TDataModel> FindOne(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters);

        Task<TDataModel> FindById<TU>(TU id);

        Task<IEnumerable<TDataModel>> FindById<TU>(IEnumerable<TU> ids);

        Task<IEnumerable<TDataModel>> FindBySqlQuery(string sql, params object[]? parameters);

        Task<QueryResult<TDataModel>> ExecuteQuery(string sql, params object[]? parameters);

        Task<TDataModel> Add(TDataModel entity);

        Task<bool> Add(IEnumerable<TDataModel> entities);

        Task<bool> RemoveById<TU>(TU id);

        Task<bool> RemoveById<TU>(IEnumerable<TU> ids);

        Task<bool> Remove(TDataModel entity);

        Task<bool> Remove(IEnumerable<TDataModel> entities);

        Task<int> Remove(Expression<Func<TDataModel, bool>> predicate, params object[]? parameters);

        Task<TDataModel> Save(TDataModel entity);

        Task<int> Save(IEnumerable<TDataModel> entities, params Expression<Func<TDataModel, object>>[]? expressions);

        Task<bool> Update(TDataModel entity);

        Task<bool> Update(IEnumerable<TDataModel> entities);

        Task<SaveResult<TU>> ExecuteSave<TU>(string sql, params object[]? parameters);

        Task<int> ExecuteCommand(string sql, params object[] parameters);
    }
}
