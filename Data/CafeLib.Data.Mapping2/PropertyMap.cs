using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CafeLib.Core.Extensions;

namespace CafeLib.Data.Mapping2
{
    internal class PropertyMap<TModel> : IPropertyMap<TModel> where TModel : class
    {
        private readonly IDictionary<string, IMapper> _propertyMap = new ConcurrentDictionary<string, IMapper>();

        public IEnumerator<IMapper> GetEnumerator() => _propertyMap.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Find the sql property from the property info map.
        /// </summary>
        /// <param name="prop">property info</param>
        /// <returns></returns>
        public IMapper Find(PropertyInfo prop)
        {
            return _propertyMap.TryGetValue(prop.Name, out var value) ? value : default;
        }

        /// <summary>
        /// Map property info.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public IMapper Map(PropertyInfo propertyInfo)
        {
            var mapper = _propertyMap.GetOrAdd(propertyInfo.Name, new PropertyConverter(propertyInfo));
            return mapper;
        }

        /// <summary>
        /// Map property by expression.
        /// </summary>
        /// <typeparam name="TProperty">type of the property</typeparam>
        /// <param name="expression">mapping expression</param>
        /// <returns>mapper</returns>
        public IMapper Map<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var info = (PropertyInfo) GetMemberInfo(expression);
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