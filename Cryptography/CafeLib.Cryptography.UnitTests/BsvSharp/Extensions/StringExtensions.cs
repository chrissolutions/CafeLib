using CafeLib.Cryptography.UnitTests.BsvSharp.Encoding;

namespace CafeLib.Cryptography.UnitTests.BsvSharp.Extensions
{
    public static class StringExtensions
    {
        public static byte[] AsciiToBytes(this string s) => Encoders.Ascii.Decode(s);

        public static byte[] HexToBytes(this string s) => Encoders.Hex.Decode(s);

        public static byte[] Utf8ToBytes(this string s) => Encoders.Utf8.Decode(s);

        public static byte[] Utf8NormalizedToBytes(this string s) => System.Text.Encoding.UTF8.GetBytes(s.Normalize(System.Text.NormalizationForm.FormKD));
    }
}