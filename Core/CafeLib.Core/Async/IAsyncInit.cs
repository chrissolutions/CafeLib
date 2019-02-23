using System.Threading.Tasks;

namespace CafeLib.Core.Async
{
    /// <summary>
    /// Asynchronous initializer interface.
    /// </summary>
    public interface IAsyncInit<T>
    {
        /// <summary>
        /// The result of the asynchronous initialization of this instance.
        /// </summary>
        Task<T> InitAsync();
    }
}
