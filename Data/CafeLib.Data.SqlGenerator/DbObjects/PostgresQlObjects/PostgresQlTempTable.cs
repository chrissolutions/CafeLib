using CafeLib.Data.SqlGenerator.DbObjects.SqliteObjects;

namespace CafeLib.Data.SqlGenerator.DbObjects.PostgresQlObjects
{
    public class PostgresQlTempTable : SqliteTempTable
    {
        public PostgresQlTempTable()
        {
            RowNumberColumnName = TranslationConstants.PostgresQlRowNumberColumnAlias;
        }
    }
}