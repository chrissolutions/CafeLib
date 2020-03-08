using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Dto;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    public class ExpressionConverter<TEntity, TDto> : ExpressionVisitor where TEntity : class, IDataModel where TDto : MappedEntity<TDto>
    {
        private ParameterExpression _fromParameter;
        private ParameterExpression _toParameter;
        
        public IDictionary<string, ISqlProperty> PropertyMap { get; }

        public ExpressionConverter()
        {
            PropertyMap = MappedEntity<TDto>.EntityMap.Properties.ToDictionary(x => x.PropertyInfo.Name, x => x);
        }

        public override Expression Visit(Expression node)
        {
            if (_fromParameter == null)
            {
                if (node.NodeType != ExpressionType.Lambda)
                {
                    throw new ArgumentException("Expression must be a lambda");
                }

                var lambda = (LambdaExpression) node;
                if (lambda.ReturnType != typeof(bool) || lambda.Parameters.Count != 1 || lambda.Parameters[0].Type != typeof(TEntity))
                {
                    throw new ArgumentException("Expression must be a predicate of type Func<TEntity, bool>");
                }

                _fromParameter = lambda.Parameters[0];
                _toParameter = Expression.Parameter(typeof(TDto), _fromParameter.Name);
            }

            return base.Visit(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _fromParameter == node ? _toParameter : base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _fromParameter && PropertyMap.TryGetValue(node.Member.Name, out var member))
            {
                return Expression.Property(_toParameter, member.PropertyInfo);
            }

            return base.VisitMember(node);
        }
    }
}