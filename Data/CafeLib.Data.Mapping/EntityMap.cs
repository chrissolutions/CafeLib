using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CafeLib.Core.Collections;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    /// <summary>
    /// Represents a typed mapping of an entity.
    /// </summary>
    internal class EntityMap<T> : IEntityMap<T> where T : IEntity
    {
        private readonly IDictionary<string, ISqlProperty> _propertyMap;
        private readonly IDictionary<string, IDictionary<string, ISqlProperty>> _tableMap;

        public EntityMap()
        {
            _propertyMap = new ThreadSafeDictionary<string, ISqlProperty>();
            _tableMap = new ThreadSafeDictionary<string, IDictionary<string, ISqlProperty>>();
        }

        /// <summary>
        /// Get the tables listed in the table map.
        /// </summary>
        public IEnumerable<string> Tables => _tableMap.Keys;

        /// <summary>
        /// Properties supported by the entity.
        /// </summary>
        public IEnumerable<ISqlProperty> Properties => _propertyMap.Values;

        /// <summary>
        /// Find the sql property from the property info map.
        /// </summary>
        /// <param name="prop">property info</param>
        /// <returns></returns>
        public ISqlProperty Find(PropertyInfo prop)
        {
            return _propertyMap.TryGetValue(prop.Name, out var value) ? value : default;
        }

        /// <summary>
        /// Finds the sql properties associated with the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>Collection of sql properties</returns>
        public IEnumerable<ISqlProperty> FindTableProperties(string tableName)
        {
            _tableMap.TryGetValue(tableName, out var results);
            return results?.Select(x => x.Value);
        }

        /// <summary>
        /// Map property info.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public IMapper Map(PropertyInfo propertyInfo)
        {
            var sqlProperty = _propertyMap.GetOrAdd(propertyInfo.Name, new SqlProperty(propertyInfo));
            sqlProperty.Tables.ForEach(x =>
                _tableMap.GetOrAdd(x, new ConcurrentDictionary<string, ISqlProperty>())
                    .GetOrAdd(sqlProperty.PropertyInfo.Name, sqlProperty));
            return sqlProperty;
        }

        /// <summary>
        /// Map property by expression.
        /// </summary>
        /// <typeparam name="TProperty">type of the property</typeparam>
        /// <param name="expression">mapping expression</param>
        /// <returns>mapper</returns>
        public IMapper Map<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var info = (PropertyInfo)GetMemberInfo(expression);
            return Map(info);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Reflection.MemberInfo"/> for the specified lambda expression.
        /// </summary>
        /// <param name="lambda">A lambda expression containing a MemberExpression.</param>
        /// <returns>A MemberInfo object for the member in the specified lambda expression.</returns>
        private static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            Expression expr = lambda;
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;

                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;

                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression)expr;
                        var baseMember = memberExpression.Member;

                        while (memberExpression != null)
                        {
                            var type = memberExpression.Type;
                            if (type.GetMembers().Any(member => member.Name == baseMember.Name))
                            {
                                return type.GetMember(baseMember.Name).First();
                            }

                            memberExpression = memberExpression.Expression as MemberExpression;
                        }

                        // Make sure we get the property from the derived type.
                        var paramType = lambda.Parameters[0].Type;
                        return paramType.GetMember(baseMember.Name).First();

                    default:
                        return null;
                }
            }
        }
    }
}