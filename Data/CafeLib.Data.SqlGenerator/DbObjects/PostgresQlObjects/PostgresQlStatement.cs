using System;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.SqlGenerator.DbObjects.PostgresQlObjects
{
    public class PostgresQlStatement : SqlStatement
    {
        public PostgresQlStatement(IDbObject script) : base(script)
        {
        }

        public override string ToString()
        {
            return base.ToString().Trim() + ";" + Environment.NewLine;
        }
    }
}