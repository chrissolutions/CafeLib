using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Data;
using CafeLib.Data.Mapping.Visitors;
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
        private readonly DefaultExpressionVisitor _defaultVisitor;

        public ExpressionConverter()
        {
            _entityProperties = MappedEntity<TModel, TEntity>.EntityProperties;

            _propertyMap = MappedEntity<TModel, TEntity>.PropertyMap.Cast<PropertyConverter>()
                .ToDictionary(x => x.PropertyInfo.Name, x => x);

            _defaultVisitor = new DefaultExpressionVisitor();
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
            var expr = node.Body;
            var @true = true;

            while (true)
            {
                switch (expr)
                {
                    case MemberExpression member:
                        {
                            var parameterExpression = _parameterVisitor.FindParameter(member);
                            var constantExpression = Expression.Constant(@true);
                            var binaryExpression = Expression.MakeBinary(ExpressionType.Equal, parameterExpression, constantExpression);
                            expr = binaryExpression;
                            break;
                        }

                    case UnaryExpression unary:
                        {
                            switch (expr.NodeType)
                            {
                                case ExpressionType.Not:
                                    @true ^= true;
                                    expr = unary.Operand;
                                    break;

                                default:
                                    throw new NotSupportedException(nameof(expr.NodeType));
                            }

                            break;
                        }

                    case BinaryExpression binary:
                        switch (expr.NodeType)
                        {
                            case ExpressionType.AndAlso:
                            case ExpressionType.OrElse:
                                var lambda = Expression.Lambda(_defaultVisitor.Visit(binary.Left) ?? throw new InvalidOperationException(), node.Parameters.First());
                                var left = (LambdaExpression)Visit(lambda) ?? throw new InvalidOperationException();
                                lambda = Expression.Lambda(_defaultVisitor.Visit(binary.Right) ?? throw new InvalidOperationException(), node.Parameters.First());
                                var right = (LambdaExpression)Visit(lambda) ?? throw new InvalidOperationException();
                                return Expression.Lambda<Func<TEntity, bool>>(Expression.MakeBinary(expr.NodeType, left.Body, right.Body), _entityParameter);

                            default:
                                var binaryExpression = Visit(binary);
                                return Expression.Lambda<Func<TEntity, bool>>(binaryExpression ?? throw new InvalidOperationException(), _entityParameter);
                        }

                    default:
                        throw new NotSupportedException(nameof(node.Body));
                }
            }
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