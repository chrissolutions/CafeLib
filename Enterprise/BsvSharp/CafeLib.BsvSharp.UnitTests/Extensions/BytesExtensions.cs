using CafeLib.BsvSharp.Encoding;

namespace CafeLib.BsvSharp.UnitTests.Extensions
{
    public static class BytesExtensions
    {
        public static string ToHex(this byte[] a) => Encoders.Hex.Encode(a);
        public static string ToHexReverse(this byte[] a) => Encoders.HexReverse.Encode(a);
    }
}
