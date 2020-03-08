using System;
using System.Linq.Expressions;

namespace CafeLib.Data.Expressions
{
    public class Where<T, TR> : Query
    {
        private readonly Select<T, TR> _select;
        private readonly Expression<Func<T, bool>> _where;

        internal Where(Select<T, TR> select, Expression<Func<T, bool>> where)
        {
            _select = select;
            _where = where;
        }

        internal override QueryBuilder ToSql(QueryBuilder qb)
        {
            _select?.ToSql(qb);

            switch (_where.Body)
            {
                case BinaryExpression b:
                    BuildExpression(qb, b, Clause.And);
                    break;

                case MethodCallExpression call:
                    BuildLikeExpression(qb, call, Clause.And);
                    break;
            }

            return qb;
        }

        private static void BuildExpression(QueryBuilder qb, Expression expression, Func<QueryBuilder, Expression, string, Clause> clause)
        {
            switch (expression)
            {
                case BinaryExpression b:
                    BuildBinaryExpression(qb, b, clause);
                    break;

                case MethodCallExpression call:
                    BuildLikeExpression(qb, call, clause);
                    break;
            }
        }

        private static void BuildBinaryExpression(QueryBuilder qb, BinaryExpression expression, Func<QueryBuilder, Expression, string, Clause> clause)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    clause(qb, expression, "=").AppendAsBinaryExpression();
                    break;

                case ExpressionType.NotEqual:
                    clause(qb, expression, "<>").AppendAsBinaryExpression();
                    break;

                case ExpressionType.GreaterThan:
                    clause(qb, expression, ">").AppendAsBinaryExpression();
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    clause(qb, expression, ">=").AppendAsBinaryExpression();
                    break;

                case ExpressionType.LessThan:
                    clause(qb, expression, "<").AppendAsBinaryExpression();
                    break;

                case ExpressionType.LessThanOrEqual:
                    clause(qb, expression, "<=").AppendAsBinaryExpression();
                    break;

                case ExpressionType.AndAlso:
                    BuildExpression(qb, expression.Left, clause);
                    BuildExpression(qb, expression.Right, Clause.And);
                    break;

                case ExpressionType.OrElse:
                    BuildExpression(qb, expression.Left, clause);
                    BuildExpression(qb, expression.Right, Clause.Or);
                    break;

                default:
                    throw new InvalidOperationException(nameof(expression.NodeType));
            }
        }

        private static void BuildLikeExpression(QueryBuilder qb, MethodCallExpression expression, Func<QueryBuilder, Expression, string, Clause> clause)
        {
            clause(qb, expression, "LIKE").AppendAsLikeExpression();
        }
    }
}