using System;
using System.Collections.Generic;
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
        /// Concatenates two or more arrays into a single one.
        /// </summary>
        public static byte[] Concat(this byte[] b, params byte[][] args)
        {
            var arrays = new List<byte[]> {b};
            arrays.AddRange(args);
            var bytes = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, bytes, offset, array.Length);
                offset += array .Length;
            }

            return bytes;
        }
    }
}
