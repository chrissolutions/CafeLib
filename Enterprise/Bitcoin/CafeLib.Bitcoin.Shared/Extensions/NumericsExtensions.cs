using System;
using CafeLib.Bitcoin.Shared.Buffers;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class NumericsExtensions
    {
        public static ByteSpan AsSpan(this int i) => BitConverter.GetBytes(i);

        // <summary>
        // Returns access to an integer as a span of bytes.
        // Reflects the endian of the underlying implementation.
        // </summary>
        // <param name = "i" ></param >
        // < returns ></returns >
        //public static ByteSpan AsSpan(this ref Int32 i)
        //{
        //    unsafe
        //    {
        //        fixed (Int32* p = &i)
        //        {
        //            byte* pb = (byte*)p;
        //            var bytes = new Span<byte>(pb, 4);
        //            return bytes;
        //        }
        //    }
        //}

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        public static ReadOnlyByteSpan AsReadOnlySpan(this int i, bool bigEndian = false)
        {
            var bytes = BitConverter.GetBytes(i);

            if (BitConverter.IsLittleEndian == bigEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        ///// <summary>
        ///// Returns access to an integer as a span of bytes.
        ///// Reflects the endian of the underlying implementation.
        ///// </summary>
        ///// <param name="i"></param>
        ///// <param name="bigEndian"></param>
        ///// <returns></returns>
        //public static ReadOnlyByteSpan AsReadOnlySpan(this ref Int32 i, bool bigEndian = false)
        //{
        //    byte[] bytes = i.AsSpan();

        //    if (BitConverter.IsLittleEndian == bigEndian)
        //    {
        //        Array.Reverse(bytes);
        //    }

        //    return bytes;
        //}

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ByteSpan AsSpan(this ref uint i)
        {
            unsafe
            {
                fixed (uint* p = &i)
                {
                    byte* pb = (byte*)p;
                    var bytes = new Span<byte>(pb, 4);
                    return bytes;
                }
            }
        }

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ReadOnlyByteSpan AsReadOnlySpan(this ref uint i) => i.AsSpan();

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ByteSpan AsSpan(this ref Int64 i)
        {
            unsafe
            {
                fixed (Int64* p = &i)
                {
                    byte* pb = (byte*)p;
                    var bytes = new Span<byte>(pb, 8);
                    return bytes;
                }
            }
        }

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ReadOnlyByteSpan AsReadOnlySpan(this ref Int64 i) => i.AsSpan();

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this ref UInt64 i)
        {
            unsafe
            {
                fixed (UInt64* p = &i)
                {
                    byte* pb = (byte*)p;
                    var bytes = new Span<byte>(pb, 8);
                    return bytes;
                }
            }
        }

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> AsReadOnlySpan(this ref UInt64 i) => i.AsSpan();
    }
}
