using System.Reflection;

namespace Secp256k1Net
{
    internal static class SymbolNameCache<TDelegate>
    {
        public static string GetSymbolName() => typeof(TDelegate).GetCustomAttribute<SymbolNameAttribute>().Name;
    }
}