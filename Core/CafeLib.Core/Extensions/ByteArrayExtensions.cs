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
        public static string ToHex(this byte[] b)
        {
            return BitConverter.ToString(b).Replace("-", string.Empty);
        }

        /// <summary>
        /// Combines two or more byte arrays into a single one.
        /// </summary>
        public static byte[] Concat(this byte[] b, params byte[][] args)
        {
            var arrays = new byte[args.Length + 1][];
            arrays[0] = b;
            args.CopyTo(arrays.AsSpan()[1..]);
            var bytes = new byte[arrays.Sum(x => x.Length)].AsSpan();
            var offset = 0;
            foreach (var array in arrays)
            {
                array.CopyTo(bytes[offset..]);
                offset += array.Length;
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Duplicate the byte array.
        /// </summary>
        /// <param name="source">source byte array</param>
        /// <returns>copy of the source byte array</returns>
        public static byte[] Duplicate(this byte[] source)
        {
            var dup = new byte[source.Length];
            Buffer.BlockCopy(source, 0, dup, 0, source.Length);
            return dup;
        }
    }
}
