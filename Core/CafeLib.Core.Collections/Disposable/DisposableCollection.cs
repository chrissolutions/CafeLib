using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CafeLib.Core.Collections.Disposable
{
    public sealed class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable
    {
        public DisposableCollection()
        {
        }

        public DisposableCollection(IList<T> items)
            : base(items)
        {
        }

        public void Dispose()
        {
            foreach (var item in this)
            {
                try
                {
                    item.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}