using System.Data;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;
using Microsoft.Data.Sqlite;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Sources.Sqlite
{
    public class SqliteOptions : IConnectionOptions
    {
        public ISqlCommandProcessor CommandProcessor { get; } = CommandProvider.Current;
        public IDbObjectFactory DbObjectFactory { get; } = new SqlObjectFactory();
        public IDbConnection GetConnection(string connectionString) => new SqliteConnection(connectionString);
    }
}
