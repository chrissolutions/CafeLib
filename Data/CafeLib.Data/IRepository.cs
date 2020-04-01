using System.Threading.Tasks;
using CafeLib.Core.Data;

namespace CafeLib.Data
{
    public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : class, IEntity
    {
        Task<int> ExecuteCommand(string sql, object? parameters = null);
    }
}
