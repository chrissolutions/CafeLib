using System;
using System.Diagnostics;
using System.Text;
using CafeLib.BsvSharp.Buffers;

namespace CafeLib.BsvSharp.Encoding
{
    /// <summary>
    /// </summary>
    public class Base58Encoder : Encoder
    {
        /** All alphanumeric characters except for "0", "I", "O", and "l" */
        private const string Base58Characters = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static bool IsSpace(char c)
        {
            return c switch
            {
                ' ' => true,
                '\t' => true,
                '\n' => true,
                '\v' => true,
                '\f' => true,
                '\r' => true,
                _ => false
            };
        }

        public override bool TryDecode(string encoded, out byte[] bytes)
        {
            bytes = null;
            var s = encoded.AsSpan();

            while (!s.IsEmpty && IsSpace(s[0])) s = s.Slice(1);

            var zeroes = 0;
            var length = 0;

            while (!s.IsEmpty && s[0] == '1') { zeroes++; s = s.Slice(1); }

            // Allocate enough space in big-endian base256 representation.
            // log(58) / log(256), rounded up.
            var size = s.Length * 733 / 1000 + 1;
            var b256 = new byte[size];

            // Process the characters.
            while (!s.IsEmpty && !IsSpace(s[0])) {
                // Decode base58 character
                var carry = Base58Characters.IndexOf(s[0]);
                if (carry < 0)
                    return false;
                // Apply "b256 = b256 * 58 + carry".
                var i = 0;
                for (var it = 0; (carry != 0 || i < length) && it < b256.Length; it++, i++) {
                    carry += 58 * b256[it];
                    b256[it] = (byte)(carry % 256);
                    carry /= 256;
                }
                Debug.Assert(carry == 0);
                length = i;
                s = s.Slice(1);
            }
            // Skip trailing spaces.
            while (!s.IsEmpty && IsSpace(s[0])) s = s.Slice(1);
            if (!s.IsEmpty)
                return false;

            // Skip trailing zeroes in b256.
            while (length > 0 && b256[length - 1] == 0) length--;

            // Result is zeroes times zero byte followed by b256[length - 1]...b256[0]
            var vch = new byte[zeroes + length];
            var nz = zeroes;
            while (zeroes-- > 0) vch[zeroes] = 0;
            while (length-- > 0) vch[nz++] = b256[length];

            bytes = vch;
            return true;
        }

        public override string Encode(ReadOnlyByteSpan bytes)
        {
            // Skip & count leading zeroes.
            var zeroes = 0;
            var length = 0;
            while (!bytes.IsEmpty && bytes[0] == 0) { bytes = bytes.Slice(1); zeroes++; }

            // Allocate enough space in big-endian base58 representation.
            // log(256) / log(58), rounded up.
            var size = bytes.Length * 138 / 100 + 1;
            var b58 = new byte[size];

            // Process the bytes.
            while (!bytes.IsEmpty) {
                var carry = (int)bytes[0];
                var i = 0;
                // Apply "b58 = b58 * 256 + ch".
                for (var it = 0; (carry != 0 || i < length) && it < b58.Length; it++, i++) {
                    carry += 256 * b58[it];
                    b58[it] = (byte)(carry % 58);
                    carry /= 58;
                }
                Debug.Assert(carry == 0);
                length = i;
                bytes = bytes.Slice(1);
            }

            // Skip trailing zeroes in b58.
            while (length > 0 && b58[length - 1] == 0) length--;

            // Translate the result into a string.
            // Result is zeroes times "1" followed by pszBase58 indexed by b58[length - 1]...b58[0]

            var sb = new StringBuilder();
            while (zeroes-- > 0) sb.Append("1");
            while (length-- > 0) sb.Append(Base58Characters[b58[length]]);
            return sb.ToString();
        }
    }
}