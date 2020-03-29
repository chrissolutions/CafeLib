namespace CafeLib.Data.Sources.SqlServer
{
    public class SqlServerOptions : IConnectionOptions
    {
        public ISqlCommandProcessor CommandProcessor { get; } = new SqlServerCommandProcessor();
    }
}
