using System;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.UnitTest.DbObjects
{
    public class SqliteObjectFactory : SqlObjectFactory
    {
        public SqliteObjectFactory()
        {
            OutputOption = new DbOutputOption
            {
                QuotationMark = "'"
            };
        }

        public override IDbStatment BuildStatement(IDbObject script)
        {
            return new SqliteStatement(script);
        }

        public override IDbTempTable BuildTempTable(string tableName, IDbSelect sourceSelect = null)
        {
            return new SqliteTempTable
            {
                TableName = tableName,
                SourceSelect = sourceSelect,
                OutputOption = OutputOption
            };
        }

        public override IDbFunc BuildFunc(string name, bool isAggregation, Type type, params IDbObject[] parameters)
        {
            return new SqliteFunc(name, type, parameters)
            {
                IsAggregation = isAggregation,
                OutputOption = OutputOption
            };
        }
        
        public override DbLimit BuildLimit(int fetch, int offset = 0)
        {
            return new SqliteLimit(offset, fetch);
        }
    }
}