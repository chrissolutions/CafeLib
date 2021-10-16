using CafeLib.Core.Encodings;

namespace CafeLib.Core.Numerics
{
    internal static class Encoders
    {
        public static HexEncoder Hex = new HexEncoder();
        public static HexReverseEncoder HexReverse = new HexReverseEncoder();
    }
}
