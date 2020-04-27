using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    public abstract class MappedEntity<TModel, TEntity> : IMappedEntity<TModel, TEntity> where TModel : class, IMappedEntity<TModel, TEntity> where TEntity : class, IEntity
    {
        public static readonly IPropertyMap<TModel> PropertyMap = new PropertyMap<TModel>();
        internal static readonly PropertyDictionary<TEntity> EntityProperties = new PropertyDictionary<TEntity>();

        private readonly IDictionary<string, PropertyConverter> _propertyConverters;

        static MappedEntity()
        {
            typeof(TModel).GetProperties().ForEach(PropertyMap.Map);
        }

        protected MappedEntity()
        {
            _propertyConverters = PropertyMap.Cast<PropertyConverter>().ToDictionary(p => p.PropertyInfo.Name);
        }

        /// <summary>
        /// Map property by expression.
        /// </summary>
        /// <typeparam name="TProperty">type of the property</typeparam>
        /// <param name="expression">mapping expression</param>
        /// <returns>mapper</returns>
        protected IMapper Map<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return PropertyMap.Map(expression);
        }

        /// <summary>
        /// Copy values from data model to mapped entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>mapped entity</returns>
        public TEntity ToEntity(TEntity entity = default)
        {
            var dto = entity ?? Activator.CreateInstance<TEntity>();

            foreach (var (_, propertyConverter) in _propertyConverters)
            {
                var entityProperty = EntityProperties[propertyConverter.PropertyInfo.Name];
                if (entityProperty == null || !entityProperty.CanRead || entityProperty.GetGetMethod() == null) continue;
                var value = propertyConverter.ToObject != null
                            ? ((Delegate) propertyConverter.ToObject).DynamicInvoke(propertyConverter.PropertyInfo.GetValue(this))
                            : propertyConverter.PropertyInfo.GetValue(this);

                entityProperty.SetValue(dto, value);
            }

            return dto;
        }

        /// <summary>
        /// Copy values to data model from mapped entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>data model</returns>
        public TModel Populate(TEntity entity)
        {
            var dto = entity ?? throw new ArgumentNullException(nameof(entity));

            foreach (var entityProperty in EntityProperties.Values)
            {
                var propertyConverter = _propertyConverters[entityProperty.Name];
                var modelProperty = propertyConverter?.PropertyInfo;
                if (modelProperty == null || !modelProperty.CanWrite || modelProperty.GetSetMethod() == null) continue;

                var value = propertyConverter.ToProperty != null
                    ? ((Delegate)propertyConverter.ToProperty).DynamicInvoke(entityProperty.GetValue(dto))
                    : entityProperty.GetValue(dto);

                modelProperty.SetValue(this, value);
            }

            return (TModel)(object)this;
        }
    }
}