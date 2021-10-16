using System.Data;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.Sources
{
    public interface IConnectionOptions
    {
        ISqlCommandProvider CommandProvider { get; }
        IDbObjectFactory DbObjectFactory { get; }
        IDbConnection GetConnection(string connectionString);
    }
}
