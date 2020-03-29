using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Data;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Options
{
    public class PropertyExpressionList<T> : List<Expression<Func<T, object>>> where T : IEntity
    {
        private readonly Domain _domain;

        public PropertyExpressionList(Domain domain, IEnumerable<Expression<Func<T, object>>> expressions)
        {
            _domain = domain;
            AddRange(expressions);
        }

        internal IEnumerable<string> GetPropertyNames()
        {
            return this.Select(GetMemberName);
        }

        public IEnumerable<string> GetColumnNames()
        {
            return this.Select(x => _domain.PropertyCache.GetColumnNamesCache<T>()[GetMemberName(x)]);
        }

        /// <summary>
        /// Get the property name in the lambda expression.
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        private static string GetMemberName(LambdaExpression lambda)
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
                        return memberExpression.Member.Name;
                }
            }
        }
    }
}