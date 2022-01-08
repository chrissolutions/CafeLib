namespace CafeLib.Core.Collections
{
    internal static class HeapLock
    {
        public static readonly object Mutex = new();
    }
}
