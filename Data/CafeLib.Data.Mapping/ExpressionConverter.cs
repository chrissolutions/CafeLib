using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Data;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    public class ExpressionConverter<TModel, TEntity> : ExpressionVisitor where TModel : MappedClass<TModel, TEntity> where TEntity : class, IEntity
    {
        private ParameterExpression _fromParameter;
        private ParameterExpression _toParameter;
        private readonly IDictionary<string, PropertyConverter> _propertyMap;

        public ExpressionConverter()
        {
            _propertyMap = MappedClass<TModel, TEntity>.PropertyMap.Cast<PropertyConverter>().ToDictionary(x => x.PropertyInfo.Name, x => x);
        }

        public override Expression Visit(Expression node)
        {
            if (_fromParameter == null)
            {
                if (node.NodeType != ExpressionType.Lambda)
                {
                    throw new ArgumentException("Expression must be a lambda");
                }

                var lambda = (LambdaExpression)node;
                if (lambda.ReturnType != typeof(bool) || lambda.Parameters.Count != 1 || lambda.Parameters[0].Type != typeof(TEntity))
                {
                    throw new ArgumentException("Expression must be a predicate of type Func<TEntity, bool>");
                }

                _fromParameter = lambda.Parameters[0];
                _toParameter = Expression.Parameter(typeof(TEntity), _fromParameter.Name);
            }

            return base.Visit(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _fromParameter == node ? _toParameter : base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _fromParameter && _propertyMap.TryGetValue(node.Member.Name, out var member))
            {
                return Expression.Property(_toParameter, member.PropertyInfo);
            }

            return base.VisitMember(node);
        }
    }
}