using System;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Extensions;
using CafeLib.Data.Dto;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    public abstract class MappedEntity<T> : IEntity where T : class, IEntity
    {
        public static readonly IEntityMap<T> EntityMap = new EntityMap<T>();

        static MappedEntity()
        {
            typeof(T).GetProperties().ForEach(EntityMap.Map);
        }

        ///
        public IMapper Map<TU>(Expression<Func<T, TU>> expression)
        {
            return EntityMap.Map(expression);
        }

        /// <summary>
        /// Copy values to data model from mapped entity.
        /// </summary>
        /// <typeparam name="TModel">data model type</typeparam>
        /// <param name="model">data model</param>
        /// <returns>data model</returns>
        public TModel ToModel<TModel>(TModel model) where TModel : class, IDataModel
        {
            var propertyMap = EntityMap.Properties.Cast<SqlProperty>()
                .ToDictionary(x => x.PropertyInfo.Name,
                    x => x.ConvertTo != null
                        ? x.ConvertTo.Invoke(x.PropertyInfo.GetValue(this))
                        : x.PropertyInfo.GetValue(this));

            var setters = typeof(TModel).GetProperties()
                .Where(p => p.CanWrite && p.GetSetMethod() != null)
                .ToDictionary(p => p.Name);

            foreach (var (key, value) in propertyMap)
            {
                if (setters.TryGetValue(key, out var setter))
                {
                    setter.SetValue(model, value);
                }
            }

            return model;
        }

        /// <summary>
        /// Copy values from data model to mapped entity.
        /// </summary>
        /// <typeparam name="TModel">type of data model</typeparam>
        /// <param name="model">input entity</param>
        /// <returns>mapped entity</returns>
        public MappedEntity<T> FromModel<TModel>(TModel model) where TModel : class, IDataModel
        {
            var getters = model.GetType().GetProperties()
                .Where(p => p.CanRead && p.GetGetMethod() != null)
                .ToDictionary(p => p.Name);

            foreach (var property in EntityMap.Properties.Cast<SqlProperty>())
            {
                if (getters.TryGetValue(property.PropertyInfo.Name, out var getter))
                {
                    var value = property.ConvertFrom != null
                        ? property.ConvertFrom.Invoke(getter.GetValue(model))
                        : getter.GetValue(model);

                    property.PropertyInfo.SetValue(this, value);
                }
            }

            return this;
        }
    }
}