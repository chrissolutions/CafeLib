using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Async
{
    /// <summary>
    /// Asynchronous initializer interface.
    /// </summary>
    public static class AsyncTask
    {
        /// <summary>
        // Runs an async task from synchronous environment.
        /// </summary>
        /// <param name="func">async function.</param>
        public static void Run(Func<Task> func)
        {
            var function = func ?? (() => Task.CompletedTask);
            Task.Run(async () => await function.Invoke());
        }
    }
}
