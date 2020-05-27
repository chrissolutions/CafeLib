using System.Threading;

namespace CafeLib.Mobile.Support
{
    internal class ThreadSafeBool
    {
        private long _backing;

        public ThreadSafeBool()
        {
            _backing = 0L;
        }

        public ThreadSafeBool(bool initValue)
        {
            _backing = initValue ? 1L : 0L;
        }

        public bool Get() => Interlocked.Read(ref _backing) == 1L;

        public void Set(bool value)
        {
            if (value) 
                Interlocked.CompareExchange(ref _backing, 1L, 0L);
            else
                Interlocked.CompareExchange(ref _backing, 0L, 1L);
        }

        public static implicit operator bool(ThreadSafeBool target)
        {
            return target.Get();
        }
    }
}
