using CafeLib.Core.Data;

namespace CafeLib.Data.Sources
{
    public interface IConnectionInfo
    {
        string ConnectionString { get; }
        Domain Domain { get; }
        IConnectionOptions Options { get; }
    }
}
