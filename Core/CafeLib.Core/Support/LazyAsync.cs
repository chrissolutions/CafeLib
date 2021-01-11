using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CafeLib.Core.Support
{
    public class LazyAsync<T> : Lazy<Task<T>>
    {
        /// <summary>
        /// LazyAsync constructor.
        /// </summary>
        /// <param name="valueFactory">value factory</param>
        public LazyAsync(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        {
        }

        /// <summary>
        /// LazyAsync constructor.
        /// </summary>
        /// <param name="valueFactory">value factory task</param>
        public LazyAsync(Func<Task<T>> valueFactory) :
            base(async () => await valueFactory().ConfigureAwait(false))
        {
        }

        /// <summary>
        /// Returns the awaiter for the lazy task.
        /// </summary>
        /// <returns>task awaiter</returns>
        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
    }
}