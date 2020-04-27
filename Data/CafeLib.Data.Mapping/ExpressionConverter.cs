using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public ExpressionConverter()
        {
            _propertyMap = MappedEntity<TModel, TEntity>.PropertyMap.Cast<PropertyConverter>()
                .ToDictionary(x => x.PropertyInfo.Name, x => x);

            _entityProperties = MappedEntity<TModel, TEntity>.EntityProperties;
        }

        public Expression<Func<TEntity, bool>> Convert(Expression<Func<TModel, bool>> expression)
        {
            return (Expression<Func<TEntity, bool>>) Visit(expression);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (typeof(T) != typeof(Func<TModel, bool>)) return base.VisitLambda(node);

            _entityParameter = Expression.Parameter(typeof(TEntity), node.Parameters.First().Name);
            return Expression.Lambda<Func<TEntity, bool>>(Visit(node.Body) ?? throw new InvalidOperationException(), _entityParameter);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Type == typeof(TModel) ? _entityParameter : base.VisitParameter(node);
        }

        //public override Expression Visit(Expression node)
        //{
        //    if (_fromParameter == null)
        //    {
        //        if (node.NodeType != ExpressionType.Lambda)
        //        {
        //            throw new ArgumentException("Expression must be a lambda");
        //        }

        //        var lambda = (LambdaExpression)node;
        //        if (lambda.ReturnType != typeof(bool) || lambda.Parameters.Count != 1 || lambda.Parameters[0].Type != typeof(TModel))
        //        {
        //            throw new ArgumentException("Expression must be a predicate of type Func<TModel, bool>");
        //        }

        //        _fromParameter = lambda.Parameters[0];
        //        _toParameter = Expression.Parameter(typeof(TEntity), _fromParameter.Name);
        //    }

        //    return base.Visit(node);
        //}

        protected override Expression VisitMember(MemberExpression node)
        {
            Expression expr = node;
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Parameter:
                        return Expression.MakeMemberAccess(Visit(node.Expression), _entityProperties[node.Member.Name]);



                    //case ExpressionType.Constant:
                    //    return base.VisitConstant((ConstantExpression)expr);
                    case ExpressionType.MemberAccess:
                        expr = ((MemberExpression)expr).Expression;
                        break;
                    default:
                        return base.VisitMember(node);
                }
            }

            //if (node.Member.DeclaringType == typeof(TModel))
            //{
            //    var member = typeof(TEntity).GetMember(node.Member.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).FirstOrDefault();
            //    if (member == null)
            //        throw new InvalidOperationException("Cannot identify corresponding member of DataObject");
            //    return Expression.MakeMemberAccess(Visit(node.Expression), member);
            //}
            //return base.VisitMember(node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            //this.expressions.Add(node);
            return base.VisitRuntimeVariables(node);
        }

        //protected override Expression VisitMember(MemberExpression node)
        //{
        //    if (node.Member.DeclaringType != typeof(TModel)) return base.VisitMember(node);

        //    Expression expr = node;
        //    while (true)
        //    {
        //        switch (expr.NodeType)
        //        {
        //            case ExpressionType.Parameter:
        //                if (!_propertyMap.TryGetValue(node.Member.Name, out var member)) throw new InvalidOperationException(nameof(TEntity));
        //                return Expression.Property(_entityParameter, MappedEntity<TModel, TEntity>.EntityProperties[member.PropertyInfo.Name]);

        //            case ExpressionType.MemberAccess:
        //                expr = ((MemberExpression)expr).Expression;
        //                break;

        //            default:
        //                return base.VisitMember(node);
        //        }
        //    }


        //if (node.Expression == _fromParameter && _propertyMap.TryGetValue(node.Member.Name, out var member))
        //{
        //    var property = Expression.Property(_toParameter, MappedEntity<TModel, TEntity>.EntityProperties[member.PropertyInfo.Name]);
        //    return property;
        //    //return Expression.Property(_toParameter, member.PropertyInfo);
        //}

        //return base.VisitMember(node);
    }
}