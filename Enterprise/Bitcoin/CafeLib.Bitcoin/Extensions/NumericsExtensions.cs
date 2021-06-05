using System;
using System.Runtime.CompilerServices;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Extensions
{
    public static class NumericsExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
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
                    var bytes = new Span<byte>(pb, sizeof(int));
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
        public static ByteSpan AsSpan(this ref long i)
        {
            unsafe
            {
                fixed (long* p = &i)
                {
                    var pb = (byte*)p;
                    var bytes = new Span<byte>(pb, sizeof(long));
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
        public static ReadOnlyByteSpan AsReadOnlySpan(this ref long i) => i.AsSpan();

        /// <summary>
        /// Returns access to an integer as a span of bytes.
        /// Reflects the endian of the underlying implementation.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Span<byte> AsSpan(this ref ulong i)
        {
            unsafe
            {
                fixed (ulong* p = &i)
                {
                    byte* pb = (byte*)p;
                    var bytes = new Span<byte>(pb, sizeof(ulong));
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
        public static ReadOnlySpan<byte> AsReadOnlySpan(this ref ulong i) => i.AsSpan();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static UInt256 AsUInt256(this byte[] bytes) => (UInt256)(ByteSpan)bytes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static byte[] AsVarIntBytes(this int v) => ((Variant)v).ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static byte[] AsVarIntBytes(this long v) => ((Variant)v).ToArray();

        /// <summary>
        /// Copy to UInt160 from byte array.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this byte[] source, ref UInt160 destination)
        {
            ((ReadOnlyByteSpan)source).CopyTo(destination.Span);
        }

        /// <summary>
        /// Copy to UInt256 from byte array.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this byte[] source, ref UInt256 destination)
        {
            ((ReadOnlyByteSpan)source).CopyTo(destination.Span);
        }

        /// <summary>
        /// Copy to UInt512 from byte array.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this byte[] source, ref UInt512 destination)
        {
            ((ReadOnlyByteSpan)source).CopyTo(destination.Span);
        }
    }
}
