using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.SqlGenerator.DbObjects.MySqlObjects
{
    public class MySqlObjectFactory : SqlObjectFactory
    {
        public MySqlObjectFactory()
        {
            OutputOption = new DbOutputOption
            {
                QuotationMark = "`"
            };
        }
        
        public override IDbTempTable BuildTempTable(string tableName, IDbSelect sourceSelect = null)
        {
            var sqlTable = new MySqlTempTable
            {
                TableName = tableName,
                SourceSelect = sourceSelect,
                OutputOption = OutputOption
            };

            return sqlTable;
        }

        public override DbLimit BuildLimit(int fetch, int offset = 0)
        {
            return new MySqlLimit(offset, fetch);
        }
    }
}