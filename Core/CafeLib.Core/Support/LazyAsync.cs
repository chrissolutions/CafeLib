using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Support
{
    public class LazyAsync<T> : Lazy<Task<T>>
    {
        public LazyAsync(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        {
        }

        public LazyAsync(Func<Task<T>> taskFactory) :
            base(async () => await taskFactory().ConfigureAwait(false))
        {
        }
    }
}