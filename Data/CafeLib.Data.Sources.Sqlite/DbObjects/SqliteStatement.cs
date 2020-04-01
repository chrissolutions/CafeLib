using System;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.Sources.Sqlite.DbObjects
{
    public class SqliteStatement : SqlStatement
    {
        public SqliteStatement(IDbObject script) : base(script)
        {
        }

        public override string ToString()
        {
            return base.ToString().Trim() + ";" + Environment.NewLine;
        }
    }
}