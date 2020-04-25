using CafeLib.Core.Data;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    public interface IMappedEntity<out TModel, TEntity> : IEntity where TModel : class, IMappedEntity<TModel, TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Copy values from data model to mapped entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>mapped entity</returns>
        TEntity ToEntity(TEntity entity = default);

        /// <summary>
        /// Copy values to data model from mapped entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>data model</returns>
        TModel Populate(TEntity entity);
    }
}