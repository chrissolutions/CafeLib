using System;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.SqlGenerator.DbObjects.SqliteObjects
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