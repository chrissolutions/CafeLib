using System;
using System.Linq;
using CafeLib.Core.Buffers;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.Encodings
{
    /// <summary>
    /// Base58 encoder.
    /// </summary>
    public class Base58Encoder : IEncoder
    {
        /** All alphanumeric characters except for "0", "I", "O", and "l" */
        private const string Base58Characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private static readonly char EncodedZero = Base58Characters[0];
        private static readonly int[] Indexes = new int[128];

        public Base58Encoder()
        {
            Array.Fill(Indexes, -1);
            Base58Characters.ForEach((x, i) => Indexes[x] = i);
        }

        public byte[] Decode(string source)
        {
            return TryDecode(source, out var bytes)
                ? bytes
                : throw new FormatException(nameof(source));
        }

        public bool TryDecode(string encoded, out byte[] bytes)
        {
            bytes = Array.Empty<byte>();
            if (encoded.Length == 0) return true;

            // Convert the base58-encoded ASCII chars to a base58 byte sequence (base58 digits).
            var source58 = new byte[encoded.Length];
            for (var i = 0; i < encoded.Length; ++i)
            {
                var c = encoded[i];
                var digit = c < (char)128 ? Indexes[c] : -1;
                if (digit < 0) return false;
                source58[i] = (byte)digit;
            }

            // Count leading zeros.
            var zeros = source58.TakeWhile(x => x == 0).Count();

            // Convert base-58 digits to base-256 digits.
            var decoded = new byte[encoded.Length];
            var outputStart = decoded.Length;
            for (var sourceStart = zeros; sourceStart < source58.Length;)
            {
                decoded[--outputStart] = DivMod(source58, sourceStart, 58, 256);
                if (source58[sourceStart] == 0)
                {
                    ++sourceStart; // optimization - skip leading zeros
                }
            }

            // Ignore extra leading zeroes that were added during the calculation.
            while (outputStart < decoded.Length && decoded[outputStart] == 0)
            {
                ++outputStart;
            }

            // Return decoded data (including original number of leading zeros).
            bytes = decoded[(outputStart - zeros)..decoded.Length];
            return true;
        }

        public string Encode(byte[] source)
        {
            if (source.Length == 0) return "";

            // Count leading zeros.
            var zeros = source.TakeWhile(x => x == 0).Count();

            // Convert base-256 digits to base-58 digits (plus conversion to ASCII characters)
            var span = new ByteSpan(source.Duplicate());
            var encoded = new char[source.Length * sizeof(char)]; // upper bound
            var outputStart = encoded.Length;
            for (var inputStart = zeros; inputStart < span.Length;)
            {
                encoded[--outputStart] = Base58Characters[DivMod(span, inputStart, 256, 58)];
                if (span[inputStart] == 0)
                {
                    ++inputStart; // optimization - skip leading zeros
                }
            }

            // Preserve exactly as many leading encoded zeros in output as there were leading zeros in input.
            while (outputStart < encoded.Length && encoded[outputStart] == EncodedZero)
            {
                ++outputStart;
            }

            while (--zeros >= 0)
            {
                encoded[--outputStart] = EncodedZero;
            }
            // Return encoded string (including encoded leading zeros).
            return new string(encoded, outputStart, encoded.Length - outputStart);
        }

        /// <summary>
        /// Divides a number, represented as an array of bytes each containing a single digit
        /// in the specified base, by the given divisor. The given number is modified in-place
        /// to contain the quotient, and the return value is the remainder.
        /// </summary>
        /// <param name="number"> the number to divide </param>
        /// <param name="firstDigit"> the index within the array of the first non-zero digit
        ///        (this is used for optimization by skipping the leading zeros) </param>
        /// <param name="base"> the base in which the number's digits are represented (up to 256) </param>
        /// <param name="divisor"> the number to divide by (up to 256) </param>
        /// <returns> the remainder of the division operation </returns>
        private static byte DivMod(ByteSpan number, int firstDigit, int @base, int divisor)
        {
            // this is just long division which accounts for the base of the input digits
            var remainder = 0;
            for (var i = firstDigit; i < number.Length; i++)
            {
                var digit = number[i] & 0xFF;
                var temp = remainder * @base + digit;
                number[i] = (byte)(temp / divisor);
                remainder = temp % divisor;
            }

            return (byte)remainder;
        }
    }
}