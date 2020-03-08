using System.ComponentModel.DataAnnotations.Schema;

namespace CafeLib.Data.Mapping
{
    public class SqlSchema
    {
        public string Table { get; }

        public string Column { get; }

        public bool Key { get; }

        public bool Identity { get; }

        public bool Scoped { get; }

        public SqlSchema(string table, string column, bool key = false, bool identity = false, bool scoped = false)
        {
            Table = table;
            Column = column;
            Key = key;
            Identity = identity;
            Scoped = scoped;
        }

        internal SqlSchema(SchemaAttribute attr)
            : this(attr.Table, attr.Column, attr.Key, attr.Identity, attr.Scoped)
        {
        }

        internal SqlSchema(ColumnAttribute attr)
            : this(null, attr.Name)
        {
        }
    }
}