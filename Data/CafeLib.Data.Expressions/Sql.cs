using System;
using System.Linq.Expressions;
using CafeLib.Dto;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Expressions
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class Sql
    {
        public static Select<T, TR> Select<T, TR>(Expression<Func<T, TR>> selector) where T : IEntity => 
            Select(selector, new Table<T>());

        public static Select<T, TR> Select<T, TR>(Expression<Func<T, TR>> selector, string tableName) where T : IEntity =>
            Select(selector, new Table<T>(tableName));

        public static Select<T1, T2, TR> Select<T1, T2, TR>(Expression<Func<T1, T2, TR>> selector, Expression<Func<T1, T2, bool>> on, string tableName = null)
        {
            throw new NotImplementedException("TODO");
            //return new Select<T1, T2, TR>(selector, on, null, new Table<T1> { Name = tableName });
        }

        public static Select<T, TR> Top<T, TR>(Expression<Func<T, TR>> selector, int take, string tableName = null) where T : IEntity
        {
            return Create(selector, take, tableName);
        }

        private static Select<T, TR> Create<T, TR>(Expression<Func<T, TR>> selector, int? take, string tableName) where T : IEntity
        {
            return Create(selector, take, new Table<T>(tableName));
        }

        public static Select<T, TR> Select<T, TR>(Expression<Func<T, TR>> selector, Table<T> table) where T : IEntity
        {
            return Create(selector, null, table);
        }

        public static Select<T, TR> Top<T, TR>(Expression<Func<T, TR>> selector, int take, Table<T> table) where T : IEntity
        {
            return Create(selector, take, table);
        }

        public static Select<T, TR> Select<T, TR>(Expression<Func<T, TR>> selector, Table table)
        {
            return Create(selector, null, table);
        }

        public static Select<T, TR> Top<T, TR>(Expression<Func<T, TR>> selector, int take, Table table)
        {
            return Create(selector, take, table);
        }

        public static Where<T, TR> Where<T, TR>(Expression<Func<T, bool>> predicate)
        {
            return new Where<T, TR>(null, predicate);
        }

        public static Table<T> Table<T>(string tableName) where T : IEntity
        {
            return new Table<T>(tableName);
        }

        private static Select<T, TR> Create<T, TR>(Expression<Func<T, TR>> selector, int? take, Table table)
        {
            return new Select<T, TR>(selector, take, table);
        }
    }
}