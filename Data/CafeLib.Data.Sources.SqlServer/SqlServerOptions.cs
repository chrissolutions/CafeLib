using System.Data;
using Microsoft.Data.SqlClient;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Sources.SqlServer
{
    public class SqlServerOptions : IConnectionOptions
    {
        public ISqlCommandProvider CommandProvider { get; } = SqlServerCommandProvider.Current;
        public IDbObjectFactory DbObjectFactory { get; } = new SqlObjectFactory();
        public IDbConnection GetConnection(string connectionString) => new SqlConnection(connectionString);
    }
}
