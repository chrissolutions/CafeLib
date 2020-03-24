using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using CafeLib.Core.Extensions;
using CafeLib.Data.Dto;
using CafeLib.Data.Dto.Cache;

namespace CafeLib.Data.SqlGenerator.Models
{
    public class EntityModelInfoProvider : IModelInfoProvider
    {
        private readonly ConcurrentDictionary<Type, EntityInfo> _entityInfos =
            new ConcurrentDictionary<Type, EntityInfo>();

        private readonly ConcurrentDictionary<MemberInfo, EntityFieldInfo> _fieldInfos =
            new ConcurrentDictionary<MemberInfo, EntityFieldInfo>();

        public EntityModelInfoProvider(DtoContext context)
        {
            context.GetEntityTypes().ForEach(x =>
            {
                var entityInfo = new EntityInfo {Namespace = string.Empty, EntityName = TableCache.TableName(x), Type = x};
                _entityInfos.TryAdd(x, entityInfo);

                entityInfo.Keys = PropertyCache.KeyPropertiesCache(x).Select((k, i) => new EntityFieldInfo 
                    {
                        ClrProperty = k,
                        PropertyName = k.Name,
                        DbName = k.Name,
                        ValType = k.PropertyType,
                        IsPrimaryKey = i == 0,
                        Entity = entityInfo
                    }).ToList();

                entityInfo.Columns = PropertyCache.TypePropertiesCache(x).Select(p => new EntityFieldInfo
                    {
                        ClrProperty = p,
                        PropertyName = p.Name,
                        DbName = p.Name,
                        ValType = p.PropertyType,
                        Entity = entityInfo
                    }).ToList();

                foreach (var fieldInfo in entityInfo.Keys.Concat(entityInfo.Columns))
                    _fieldInfos.TryAdd(fieldInfo.ClrProperty, fieldInfo);
            });


        }

        //private void UpdateKeysAndColumns(IEntityType et, EntityInfo ei)
        //{
        //    ei.Keys = et.GetKeys().First(k => k.IsPrimaryKey()).ToEntityFieldInfo(ei).ToList();
        //    ei.Columns = et.GetProperties().Select(p => p.ToEntityFieldInfo(ei)).ToList();

        //    foreach (var fieldInfo in ei.Keys.Concat(ei.Columns))
        //        _fieldInfos.TryAdd(fieldInfo.ClrProperty, fieldInfo);
        //}

        //private void UpdateParentRelations(IEntityType et, EntityInfo ei)
        //{
        //    foreach (var relation in et.GetForeignKeys())
        //    {
        //        if (relation.DependentToPrincipal == null)
        //        {
        //            continue;
        //        }

        //        var relationKey = relation.DependentToPrincipal.Name;

        //        // child entity in parent relation is the current entity
        //        var declaringEntity = GetOrAddEntityInfo(relation.DeclaringEntityType);
        //        var declaringKeys = relation.Properties.Select(p => p.ToEntityFieldInfo(declaringEntity));

        //        // parent entity in parent relation is the referring entity 
        //        var principalEntity = GetOrAddEntityInfo(relation.PrincipalEntityType);
        //        var principalKeys = relation.PrincipalKey.Properties.Select(p => p.ToEntityFieldInfo(principalEntity));

        //        var entityRelation = new EntityRelation
        //        {
        //            FromEntity = declaringEntity,
        //            ToEntity = principalEntity,
        //            FromProperty = relation.DependentToPrincipal.PropertyInfo,
        //            ToProperty = relation.PrincipalToDependent?.PropertyInfo,
        //            FromKeys = declaringKeys.ToList(),
        //            ToKeys = principalKeys.ToList()
        //        };

        //        ei.AddRelation(relationKey, entityRelation);
        //    }
        //}

        //private void UpdateChildRelations(IEntityType et, EntityInfo ei)
        //{
        //    foreach (var relation in et.GetReferencingForeignKeys())
        //    {
        //        if (relation.PrincipalToDependent == null)
        //        {
        //            continue;
        //        }

        //        var relationKey = relation.PrincipalToDependent.Name;

        //        // child entity in child relation is the referring entity
        //        var declaringEntity = GetOrAddEntityInfo(relation.DeclaringEntityType);
        //        var declaringKeys = relation.Properties.Select(p => p.ToEntityFieldInfo(declaringEntity));

        //        // parent entity in child relation is the current entity
        //        var principalEntity = GetOrAddEntityInfo(relation.PrincipalEntityType);
        //        var principalKeys = relation.PrincipalKey.Properties.Select(p => p.ToEntityFieldInfo(principalEntity));

        //        var entityRelation = new EntityRelation
        //        {
        //            FromEntity = principalEntity,
        //            ToEntity = declaringEntity,
        //            FromProperty = relation.PrincipalToDependent.PropertyInfo,
        //            ToProperty = relation.DependentToPrincipal?.PropertyInfo,
        //            FromKeys = principalKeys.ToList(),
        //            ToKeys = declaringKeys.ToList(),
        //            IsChildRelation = true
        //        };

        //        ei.AddRelation(relationKey, entityRelation);
        //    }
        //}

        //public EntityInfo GetOrAddEntityInfo(IEntityType et)
        //{
        //    var type = et.ClrType;
        //    if (!_entityInfos.ContainsKey(type))
        //        _entityInfos[type] = et.ToEntityInfo();
        //    return _entityInfos[type];
        //}

        public EntityInfo FindEntityInfo(Type type)
        {
            type = type.GenericTypeArguments.FirstOrDefault() ?? type;
            return _entityInfos.ContainsKey(type) ? _entityInfos[type] : null;
        }

        public EntityFieldInfo FindFieldInfo(MemberInfo memberInfo)
        {
            return _fieldInfos.ContainsKey(memberInfo) ? _fieldInfos[memberInfo] : null;
        }
    }
}