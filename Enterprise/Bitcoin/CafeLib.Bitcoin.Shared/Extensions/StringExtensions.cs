using System.Collections.Generic;
using System.Linq;
using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class StringExtensions
    {
        public static byte[] AsciiToBytes(this string s) => System.Text.Encoding.ASCII.GetBytes(s);

        public static byte[] HexToBytes(this string s) => Encoders.Hex.Decode(s);

        public static byte[] Utf8ToBytes(this string s) => System.Text.Encoding.UTF8.GetBytes(s);

        public static byte[] Utf8NormalizedToBytes(this string s) => System.Text.Encoding.UTF8.GetBytes(s.Normalize(System.Text.NormalizationForm.FormKD));
    }
}
