using System.Data;

namespace CafeLib.Data.Sources
{
    public interface IConnectionOptions
    {
        ISqlCommandProcessor CommandProcessor { get; }
        IDbConnection GetConnection();
    }
}
