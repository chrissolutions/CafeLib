using System.Collections.Generic;
using CafeLib.Data.Options;

// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Data
{
    public interface IConnectionInfo
    {
        string ConnectionUri { get; }
        string ConnectionName { get; }
        string ConnectionScheme { get; }
        string ConnectionString { get; }
        int ConnectionTimeout { get; }
        string HostName { get; }
        int Port { get; }
        string UserName { get; }
        string Password { get; }
        IReadOnlyDictionary<string, string> Parameters { get; }
        IConnectionOptions Options { get; }
    }
}
