namespace CafeLib.Data.Options.SqlServer
{
    public class SqlServerOptions : IConnectionOptions
    {
        public ISqlCommandProcessor CommandProcessor { get; } = new SqlServerCommandProcessor();
    }
}
