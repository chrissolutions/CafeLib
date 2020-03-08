using System;
using System.Linq;
using System.Linq.Expressions;

namespace CafeLib.Data.Expressions
{
    internal class Clause
    {
        private readonly Expression _expression;
        private readonly string _op;
        private readonly Func<string, string, object, string, QueryBuilder> _appendValue;

        private Clause(Expression expression, string op, Func<string, string, object, string, QueryBuilder> appendValue)
        {
            _expression = expression;
            _op = op;
            _appendValue = appendValue;
        }

        public static Clause And(QueryBuilder qb, Expression expression, string op)
        {
            return new Clause(expression, op, qb.AddCondition);
        }

        public static Clause Or(QueryBuilder qb, Expression expression, string op)
        {
            return new Clause(expression, op, qb.OrCondition);
        }

        public void AppendAsBinaryExpression()
        {
            var binaryExpression = (BinaryExpression)_expression;
            var left = binaryExpression.Left;
            var right = binaryExpression.Right;
            if (left.NodeType == ExpressionType.Constant && right.NodeType == ExpressionType.MemberAccess)
            {
                left = right;
                right = left;
            }

            var attributeName = left.NodeType switch
            {
                ExpressionType.Convert => ((MemberExpression)((UnaryExpression)left).Operand).Member.Name,
                _ => ((MemberExpression)left).Member.Name
            };

            _appendValue(_op, attributeName, EvaluateExpression(right), QueryBuilder.AliasName);
        }

        public void AppendAsLikeExpression()
        {
            var callExpression = (MethodCallExpression) _expression;
            var likeMethod = callExpression.Method.Name;
            var attributeName = ((MemberExpression)callExpression.Object)?.Member.Name;

            switch (likeMethod)
            {
                case "Contains":
                    _appendValue(_op, attributeName, $"%{EvaluateExpression(callExpression.Arguments.First())}%", QueryBuilder.AliasName);
                    break;

                case "StartWith":
                    _appendValue(_op, attributeName, $"{EvaluateExpression(callExpression.Arguments.First())}%", QueryBuilder.AliasName);
                    break;

                case "EndsWith":
                    _appendValue(_op, attributeName, $"%{EvaluateExpression(callExpression.Arguments.First())}", QueryBuilder.AliasName);
                    break;

                default:
                    throw new InvalidOperationException(nameof(attributeName));
            }
        }

        public object EvaluateExpression(Expression expression)
        {
            while (true)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Constant:
                        return ((ConstantExpression)expression).Value;

                    case ExpressionType.MemberAccess:
                        var converter = Expression.Convert((MemberExpression)expression, typeof(object));
                        return Expression.Lambda<Func<object>>(converter).Compile()();

                    case ExpressionType.Convert:
                        expression = ((UnaryExpression)expression).Operand;
                        break;

                    case ExpressionType.Call:
                        return Expression.Lambda(expression).Compile().DynamicInvoke();

                    case ExpressionType.ArrayIndex:
                        var array = EvaluateExpression(((BinaryExpression)expression).Left);
                        var index = (int)EvaluateExpression(((BinaryExpression)expression).Right);
                        return ((Array) array).GetValue(index);

                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
