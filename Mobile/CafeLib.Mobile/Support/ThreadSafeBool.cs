using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace CafeLib.Mobile.Support
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class ThreadSafeBool
    {
        private long _state;

        public ThreadSafeBool()
        {
            _state = 0L;
        }

        public ThreadSafeBool(bool initValue)
        {
            _state = initValue ? 1L : 0L;
        }

        public bool Get() => Interlocked.Read(ref _state) == 1L;

        public void Set(bool value)
        {
            if (value) 
                Interlocked.CompareExchange(ref _state, 1L, 0L);
            else
                Interlocked.CompareExchange(ref _state, 0L, 1L);
        }

        public static implicit operator bool(ThreadSafeBool target)
        {
            return target.Get();
        }
    }
}
