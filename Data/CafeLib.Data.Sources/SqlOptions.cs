using System.Data;

namespace CafeLib.Data.Sources
{
    public class SqlOptions : IConnectionOptions
    {
        public IDbConnection GetConnection() => null;

        public ISqlCommandProcessor CommandProcessor { get; } = new SqlCommandProcessor();
    }
}
