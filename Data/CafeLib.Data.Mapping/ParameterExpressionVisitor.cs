using System;
using System.Collections.Generic;
using System.Linq.Expressions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    internal class ParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly IDictionary<string, PropertyConverter> _propertyMap;

        public ParameterExpressionVisitor(IDictionary<string, PropertyConverter> propertyMap)
        {
            _propertyMap = propertyMap;
        }

        public MemberExpression FindParameter(Expression expression)
        {
            return (MemberExpression)Visit(expression);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand) ?? throw new ArgumentNullException(nameof(node.Operand));
            return operand;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Expression expr = node;
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Parameter:
                        var parameter = Expression.MakeMemberAccess(Visit(node.Expression), _propertyMap[node.Member.Name].PropertyInfo);
                        return parameter;

                    case ExpressionType.Convert:
                        var unaryExpression = (UnaryExpression)expr;
                        var operand = Visit(unaryExpression.Operand) ?? throw new ArgumentNullException(nameof(unaryExpression.Operand));
                        return operand;

                    case ExpressionType.MemberAccess:
                        expr = ((MemberExpression)expr).Expression;
                        break;

                    default:
                        return base.VisitMember(node);
                }
            }
        }
    }
}