using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Data.Dto.Cache;

// ReSharper disable UnusedParameter.Local

namespace CafeLib.Data.Expressions
{
    public class Select<T, TR> : Query
    {
        private readonly Expression<Func<T, TR>> _select;
        private readonly int? _take;
        private readonly Table _table;

        internal Select(Expression<Func<T, TR>> select, int? take, Table table)
        {
            _select = select;
            _take = take;
            _table = table;
        }

        public Where<T, TR> Where(Expression<Func<T, bool>> predicate)
        {
            return new Where<T, TR>(this, predicate);
        }

        internal override QueryBuilder ToSql(QueryBuilder qb)
        {
            qb.Select();

            if (_take.HasValue)
            {
                qb.Take(_take.Value);
            }

            var type = _select.Parameters[0].Type;

            var expressions = GetExpressions(type, _select.Body);

            AddExpressions(expressions, type, qb);

            qb.AddTable(_table);

            return qb;
        }

        private static IEnumerable<Expression> GetExpressions(Type type, Expression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.New:
                    var n = (NewExpression)body;
                    return n.Arguments;
                case ExpressionType.Parameter:
                    var propertyInfos = new SimpleTypeBinder().GetProperties(type);
                    return propertyInfos.Values.Select(pi => Expression.Property(body, pi));
                default:
                    return new[] { body };
            }
        }

        private static void AddExpressions(IEnumerable<Expression> es, Type t, QueryBuilder qb)
        {
            foreach (var e in es)
            {
                AddExpression(e, t, qb);
                qb.AddSeparator();
            }
            qb.Remove(); // Remove last comma
        }

        private static void AddExpression(Expression e, Type t, QueryBuilder qb)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Constant:
                    var c = (ConstantExpression)e;
                    qb.AddValue(c.Value);
                    break;
                case ExpressionType.MemberAccess:
                    var m = (MemberExpression)e;
                    AddExpression(m, t, qb);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void AddExpression(MemberExpression m, Type t, QueryBuilder qb)
        {
            var name = PropertyCache.GetColumnNamesCache(t)[m.Member.Name];

            //var name = m.Member.GetCustomAttributes(true)
            //    .Where(x => x.GetType() == typeof(ColumnAttribute))
            //    .Cast<ColumnAttribute>()
            //    .SingleOrDefault()?.Name 
            //           ?? m.Member.Name;

            if (m.Member.ReflectedType == t)
            {
                qb.AddAttribute(name);
            }
            else
            {
                qb.AddParameter(name);
            }
        }
    }

    public class Select<T1, T2, TR>
    {
        internal Select(Expression<Func<T1, T2, TR>> select, Expression<Func<T1, T2, bool>> on, int? take, Table table)
        {
        }

        public override string ToString()
        {
            return "";
        }
    }
}