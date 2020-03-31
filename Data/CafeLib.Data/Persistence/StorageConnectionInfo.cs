using CafeLib.Core.Data;
using CafeLib.Data.Sources;

namespace CafeLib.Data.Persistence
{
    internal class StorageConnectionInfo : IConnectionInfo
    {
        public string ConnectionString { get; }
        public Domain Domain { get; }
        public IConnectionOptions Options { get; }

        /// <summary>
        /// Storage connection info constructor.
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="domain">entity domain</param>
        /// <param name="options">connection options</param>
        public StorageConnectionInfo(string connectionString, Domain domain, IConnectionOptions? options = null)
        {
            ConnectionString = connectionString;
            Domain = domain;
            Options = options ?? new SqlOptions();
        }
    }
}