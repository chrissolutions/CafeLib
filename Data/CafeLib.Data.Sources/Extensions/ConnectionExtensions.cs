using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Data;

namespace CafeLib.Data.Sources.Extensions
{
    public static class ConnectionExtensions
    {
        public static async Task<bool> DeleteAsync<T>(this IConnectionInfo info, IEnumerable<T> data, CancellationToken token = default) where T : IEntity
        {
            using var connection = info.Options.GetConnection(info.ConnectionString);
            return await info.Options.CommandProcessor.DeleteAsync(connection, info.Domain, data, token);
        }
    }
}
