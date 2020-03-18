using CafeLib.Data.Dto;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Expressions
{
    public class Table<T> : Table where T : IEntity
    {
        public Table()
            : base(DtoContext.TableName<T>())
        {
        }

        public Table(string name)
            : base(name)
        {
        }

        public static Table<T> WithSchema(string name)
        {
            return new Table<T> { Schema = name };
        }

        public static Table<T> WithDefaultSchema()
        {
            return new Table<T>();
        }
    }

    public class Table
    {
        public Table(string name)
        {
            Name = name;
        }

        public const string AliasName = "a";

        public string Name { get; set; }

        public string Schema { get; set; } = "[dbo]";

        public string ToSql()
        {
            return $"{Schema}.[{Name}] AS {AliasName}";
        }
    }
}