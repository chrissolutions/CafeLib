using System;
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
    }
}
