using System;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts a byte array to a hexadecimal string.
        /// </summary>
        /// <param name="b">byte array</param>
        /// <returns>hexadecimal string</returns>
        public static string ToHexString(this byte[] b)
        {
            return BitConverter.ToString(b).Replace("-", string.Empty);
        }

        /// <summary>
        /// Combines two or more arrays into a single one.
        /// </summary>
        public static byte[] Combine(this byte[] b, params byte[][] args)
        {
            var arrays = new byte[args.Length + 1][];
            arrays[0] = b;
            args.CopyTo(arrays.AsSpan()[1..]);
            var bytes = new byte[arrays.Sum(x => x.Length)].AsSpan();
            var offset = 0;
            foreach (var array in arrays)
            {
                array.CopyTo(bytes[offset..]);
                offset += array .Length;
            }

            return bytes.ToArray();
        }
    }
}
