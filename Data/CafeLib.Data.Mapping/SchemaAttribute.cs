using System;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SchemaAttribute : Attribute
    {
        public SchemaAttribute(string table, string column)
        {
            Table = table;
            Column = column;
        }
        public string Table { get; }

        public string Column { get; }

        public bool Key { get; set; }

        public bool Identity { get; set; }

        public bool Scoped { get; set; }
    }
}