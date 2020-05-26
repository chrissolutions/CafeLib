using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Support
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        {
        }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(async () => await taskFactory().ConfigureAwait(false))
        {
        }
    }
}