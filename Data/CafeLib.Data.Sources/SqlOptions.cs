namespace CafeLib.Data.Sources
{
    public class SqlOptions : IConnectionOptions
    {
        public ISqlCommandProcessor CommandProcessor { get; } = new SqlCommandProcessor();
    }
}
