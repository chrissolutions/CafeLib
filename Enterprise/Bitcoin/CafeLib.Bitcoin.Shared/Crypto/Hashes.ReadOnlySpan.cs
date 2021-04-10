#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Utility;

namespace CafeLib.Bitcoin.Shared.Crypto
{
    public static partial class Hashes
    {
        public static void Ripemd160(this ReadOnlySpan<byte> data, Span<byte> hash)
        {
            Ripemd.Ripemd160(data, hash);
        }

        public static UInt160 Ripemd160(this ReadOnlySpan<byte> data)
        {
            var h = new UInt160();
            Ripemd160(data, h.Bytes);
            return h;
        }

        public static void Sha1(this ReadOnlySpan<byte> data, Span<byte> hash)
        {
            using var sha = new SHA1Managed();
            sha.TransformFinalBlock(data, hash);
        }

        public static KzUInt160 Sha1(this ReadOnlySpan<byte> data)
        {
            var hash = new KzUInt160();
            Sha1(data, hash.Span);
            return hash;
        }

        public static void Sha256(this ReadOnlySpan<byte> data, Span<byte> hash)
        {
            using var sha = new SHA256Managed();
            sha.TransformFinalBlock(data, hash);
        }

        public static KzUInt256 Sha256(this ReadOnlySpan<byte> data)
        {
            var hash = new KzUInt256();
            Sha256(data, hash.Span);
            return hash;
        }

        public static void Sha512(this ReadOnlySpan<byte> data, Span<byte> hash)
        {
            using var sha = new SHA512Managed();
            sha.TransformFinalBlock(data, hash);
        }

        public static UInt512 Sha512(this ReadOnlySpan<byte> data)
        {
            var hash = new UInt512();
            Sha512(data, hash.Span);
            return hash;
        }

        /// <summary>
        /// Computes double SHA256 of data: SHA256(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <param name="hash">Output: SHA256 of SHA256 of data.</param>
        public static void Hash256(this ReadOnlySpan<byte> data, Span<byte> hash)
        {
            var h1 = new KzUInt256();
            using var sha = new SHA256Managed();
            TransformFinalBlock(sha, data, h1.Span);
            TransformFinalBlock(sha, h1.Span, hash);
        }

        /// <summary>
        /// Computes double SHA256 of data: SHA256(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <returns>SHA256 of SHA256 of data.</returns>
        public static KzUInt256 Hash256(this ReadOnlySpan<byte> data)
        {
            var h2 = new KzUInt256();
            data.Hash256(h2.Span);
            return h2;
        }


        /// <summary>
        /// Computes RIPEMD160 of SHA256 of data: RIPEMD160(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <param name="hash">Output: RIPEMD160 of SHA256 of data.</param>
        public static void Hash160(this ReadOnlySpan<byte> data, Span<byte> hash)
        {
            var h = data.Sha256();
            Ripemd160(h.Span, hash);
        }

        /// <summary>
        /// Computes RIPEMD160 of SHA256 of data: RIPEMD160(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <returns>KzHash160 RIPEMD160 of SHA256 of data.</returns>
        public static UInt160 Hash160(this ReadOnlySpan<byte> data)
        {
            return Ripemd160(Sha256(data).Span);
        }

        public static void HmacSha256(this ReadOnlySpan<byte> key, ReadOnlySpan<byte> data, Span<byte> hash)
        {
            new HMACSHA256(key.ToArray()).TransformFinalBlock(data, hash);
        }

        public static KzUInt256 HmacSha256(this ReadOnlySpan<byte> key, ReadOnlySpan<byte> data)
        {
            var h = new KzUInt256();
            new HMACSHA256(key.ToArray()).TransformFinalBlock(data, h.Span);
            return h;
        }

        public static void HmacSha512(this ReadOnlySpan<byte> key, ReadOnlySpan<byte> data, Span<byte> hash)
        {
            new HMACSHA512(key.ToArray()).TransformFinalBlock(data, hash);
        }

        public static KzUInt512 HmacSha512(this ReadOnlySpan<byte> key, ReadOnlySpan<byte> data)
        {
            var h = new KzUInt512();
            new HMACSHA512(key.ToArray()).TransformFinalBlock(data, h.Span);
            return h;
        }

        public static byte[] ComputeHash(this HashAlgorithm alg, ReadOnlySpan<byte> buffer)
        {
            var hash = new byte[alg.HashSize];
            alg.TransformFinalBlock(buffer, hash);
            return hash;
        }

        public static void TransformFinalBlock(this HashAlgorithm alg, ReadOnlySpan<byte> data, Span<byte> hash)
        {
            var length = data.Length;
            var array = ArrayPool<byte>.Shared.Rent(Math.Min(MaxBufferSize, length));
            try {
                    var mOff = 0;
                    do {
                        var mLen = Math.Min(array.Length, data.Length - mOff);
                        data.Slice(mOff, mLen).CopyTo(array);
                        mOff += mLen;
                        if (mOff < length) {
                            alg.TransformBlock(array, 0, mLen, null, 0);
                        } else {
                            alg.TransformFinalBlock(array, 0, mLen);
                            alg.Hash.CopyTo(hash);
                        }
                    } while (mOff < length);
            }
            finally {
                Array.Clear(array, 0, array.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static void TransformBlock(this HashAlgorithm alg, ReadOnlySpan<byte> data)
        {
            var length = data.Length;
            var array = ArrayPool<byte>.Shared.Rent(Math.Min(MaxBufferSize, length));
            try {
                    var mOff = 0;
                    do {
                        var mLen = Math.Min(array.Length, data.Length - mOff);
                        data.Slice(mOff, mLen).CopyTo(array);
                        mOff += mLen;
                        alg.TransformBlock(array, 0, mLen, null, 0);
                    } while (mOff < length);
            }
            finally {
                Array.Clear(array, 0, array.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }
    }
}
