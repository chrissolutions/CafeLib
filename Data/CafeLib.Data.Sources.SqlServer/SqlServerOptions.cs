using System.Data;

namespace CafeLib.Data.Sources.SqlServer
{
    public class SqlServerOptions : IConnectionOptions
    {
        public ISqlCommandProcessor CommandProcessor { get; } = CommandProvider.Current;
        public IDbConnection GetConnection()
        {
            throw new System.NotImplementedException();
        }
    }
}
