using System.Data;
using System.Data.SQLite;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Sources.Sqlite
{
    public class SqliteOptions : IConnectionOptions
    {
        public ISqlCommandProvider CommandProvider { get; } = SqliteCommandProvider.Current;
        public IDbObjectFactory DbObjectFactory { get; } = new SqlObjectFactory();
        public IDbConnection GetConnection(string connectionString) => new SQLiteConnection(connectionString);
    }
}
