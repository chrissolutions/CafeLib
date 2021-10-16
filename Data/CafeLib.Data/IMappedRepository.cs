using CafeLib.Core.Data;
using CafeLib.Data.Mapping;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data
{
    public interface IMappedRepository<TModel, TEntity> : IRepository<TModel> where TModel : class, IMappedEntity<TModel, TEntity> where TEntity : class, IEntity
    {
    }
}
