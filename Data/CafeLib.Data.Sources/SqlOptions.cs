using System.Data;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.Sources
{
    public class SqlOptions : IConnectionOptions
    {
        public ISqlCommandProcessor CommandProcessor { get; } = new SqlCommandProcessor();
        public IDbObjectFactory DbObjectFactory { get; } = new SqlObjectFactory();
        public IDbConnection GetConnection(string connectionString) => null;
    }
}
