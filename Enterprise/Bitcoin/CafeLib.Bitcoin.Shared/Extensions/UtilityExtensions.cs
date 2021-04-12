using System.Collections.Generic;
using System.Linq;
using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class UtilityExtensions
    {
        public static int GetHashCodeOfValues(this IEnumerable<byte> bytes) => bytes?.Aggregate(17, (current, b) => current * 31 + b) ?? 0;
    }
}
