﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Data;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    public class ExpressionConverter<TModel, TEntity> : ExpressionVisitor
        where TModel : class, IMappedEntity<TModel, TEntity> where TEntity : class, IEntity
    {
        private ParameterExpression _entityParameter;
        private readonly IDictionary<string, PropertyConverter> _propertyMap;
        private readonly PropertyDictionary<TEntity> _entityProperties;
        private readonly ParameterExpressionVisitor _parameterVisitor;

        public ExpressionConverter()
        {
            _entityProperties = MappedEntity<TModel, TEntity>.EntityProperties;

            _propertyMap = MappedEntity<TModel, TEntity>.PropertyMap.Cast<PropertyConverter>()
                .ToDictionary(x => x.PropertyInfo.Name, x => x);

            _parameterVisitor = new ParameterExpressionVisitor(_propertyMap);
        }

        public Expression<Func<TEntity, bool>> Convert(Expression<Func<TModel, bool>> expression)
        {
            return (Expression<Func<TEntity, bool>>)Visit(expression);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (typeof(T) != typeof(Func<TModel, bool>)) return base.VisitLambda(node);

            _entityParameter = Expression.Parameter(typeof(TEntity), node.Parameters.First().Name);
            var bodyExpression = Visit(node.Body);
            return Expression.Lambda<Func<TEntity, bool>>(bodyExpression ?? throw new InvalidOperationException(), _entityParameter);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var leftExpression = Visit(node.Left) ?? throw new ArgumentNullException(nameof(node.Left));
            var rightExpression = VisitRight(node.Right, _parameterVisitor.FindParameter(node.Left)) ?? throw new ArgumentNullException(nameof(node.Right));
            var binaryExpression = Expression.MakeBinary(node.NodeType, leftExpression, rightExpression);
            return binaryExpression;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand) ?? throw new ArgumentNullException(nameof(node.Operand));
            return operand;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Type == typeof(TModel) ? _entityParameter : base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Expression expr = node;
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Parameter:
                        var parameter = Expression.MakeMemberAccess(Visit(node.Expression), _entityProperties[node.Member.Name]);
                        return parameter;

                    case ExpressionType.Convert:
                        var unaryExpression = (UnaryExpression)expr;
                        var operand = Visit(unaryExpression.Operand) ?? throw new ArgumentNullException(nameof(unaryExpression.Operand));
                        return operand;

                    case ExpressionType.Constant:
                        var lambdaExpression = Expression.Lambda(node).Compile();
                        var value = lambdaExpression.DynamicInvoke();
                        var converter = _propertyMap[node.Member.Name];
                        if (converter.ToObject == null) return Expression.Constant(value);
                        var converterMethod = (Delegate)converter.ToObject;
                        return Expression.Constant(converterMethod.DynamicInvoke(value));

                    case ExpressionType.MemberAccess:
                        expr = ((MemberExpression)expr).Expression;
                        break;

                    default:
                        return base.VisitMember(node);
                }
            }
        }

        private Expression VisitRight(Expression node, MemberExpression leftNode)
        {
            var expr = node;

            switch (expr.NodeType)
            {
                case ExpressionType.Constant:
                    var constantExpression = (ConstantExpression)expr;
                    var value = leftNode.Type.IsEnum ? Enum.ToObject(leftNode.Type, constantExpression.Value) : constantExpression.Value;
                    var converter = _propertyMap[leftNode.Member.Name];
                    if (converter.ToObject == null) return Expression.Constant(value);
                    var converterMethod = (Delegate)converter.ToObject;
                    return Expression.Constant(converterMethod.DynamicInvoke(value));
            }

            return Visit(expr);
        }
    }
}