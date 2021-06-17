using System;
using System.Linq;
using CafeLib.BsvSharp.Buffers;
using CafeLib.BsvSharp.Extensions;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;

namespace CafeLib.BsvSharp.Encoding
{
    /// <summary>
    /// Base58 is a way to encode Bitcoin addresses (or arbitrary data) as alphanumeric strings.
    /// <para>
    /// Note that this is not the same base58 as used by Flickr, which you may find referenced around the Internet.
    /// </para>
    /// <para>
    /// You may want to consider working with VersionedChecksummedBytes instead, which adds support for testing
    /// the prefix and suffix bytes commonly found in addresses.
    /// </para>
    /// <para>
    /// Satoshi explains: why base-58 instead of standard base-64 encoding?
    /// <ul>
    /// <li>Don't want 0OIl characters that look the same in some fonts and
    ///     could be used to create visually identical looking account numbers.</li>
    /// <li>A string with non-alphanumeric characters is not as easily accepted as an account number.</li>
    /// <li>E-mail usually won't line-break if there's no punctuation to break at.</li>
    /// <li>Doubleclicking selects the whole number as one word if it's all alphanumeric.</li>
    /// </ul>
    /// </para>
    /// <para>
    /// However, note that the encoding/decoding runs in O(n<sup>2</sup>) time, so it is not useful for large data.
    /// </para>
    /// <para>
    /// The basic idea of the encoding is to treat the data bytes as a large number represented using
    /// base-256 digits, convert the number to be represented using base-58 digits, preserve the exact
    /// number of leading zeros (which are otherwise lost during the mathematical operations on the
    /// numbers), and finally represent the resulting base-58 digits as alphanumeric ASCII characters.
    /// </para>
    /// </summary>
	public class Base58Encoder : SingletonBase<Base58Encoder>, IEncoder
    {
        public static readonly char[] Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        private static readonly char EncodedZero = Alphabet[0];
        private static readonly int[] Indexes = new int[128];

        private Base58Encoder()
        {
            Array.Fill(Indexes, -1);
            Alphabet.ForEach((x, i) => Indexes[x] = i);
        }

        public string FromHex(string source)
        {
            var bytes = Encoders.Hex.Decode(source);
            return Encode(bytes);
        }

        public string ToHex(string source)
        {
            var bytes = Decode(source);
            return Encoders.Hex.Encode(bytes);
        }

        /// <summary>
        /// Encodes the given bytes as a base58 string (no checksum is appended).
        /// </summary>
        /// <param name="source"> the bytes to encode </param>
        /// <returns> the base58-encoded string </returns>
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
                encoded[--outputStart] = Alphabet[DivMod(span, inputStart, 256, 58)];
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
        /// Decodes the given base58 string into the original data bytes.
        /// </summary>
        /// <param name="source">the base58-encoded string to decode</param>
        /// <returns> the decoded data bytes </returns>
        /// <exception cref="FormatException"> if the given string is not a valid base58 string </exception>
        public byte[] Decode(string source)
        {
            if (source.Length == 0) return Array.Empty<byte>();

            // Convert the base58-encoded ASCII chars to a base58 byte sequence (base58 digits).
            var source58 = new byte[source.Length];
            for (var i = 0; i < source.Length; ++i)
            {
                var c = source[i];
                var digit = c < (char)128 ? Indexes[c] : -1;
                if (digit < 0)
                {
                    throw new FormatException("Illegal character " + c + " at position " + i);
                }
                source58[i] = (byte)digit;
            }

            // Count leading zeros.
            var zeros = source58.TakeWhile(x => x == 0).Count();

            // Convert base-58 digits to base-256 digits.
            var decoded = new byte[source.Length];
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
            return decoded[(outputStart - zeros)..decoded.Length];
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
