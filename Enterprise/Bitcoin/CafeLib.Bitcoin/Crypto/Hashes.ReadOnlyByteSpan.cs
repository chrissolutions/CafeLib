#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Crypto
{
    public static partial class Hashes
    {
        public static void Ripemd160(this ReadOnlyByteSpan data, Span<byte> hash)
        {
            Ripemd.Ripemd160(data, hash);
        }

        public static UInt160 Ripemd160(this ReadOnlyByteSpan data)
        {
            var hash = new UInt160();
            Ripemd160(data, hash.Span);
            return hash;
        }

        public static void Sha1(this ReadOnlyByteSpan data, Span<byte> hash)
        {
            using var sha = new SHA1Managed();
            sha.TransformFinalBlock(data, hash);
        }

        public static UInt160 Sha1(this ReadOnlyByteSpan data)
        {
            var hash = new UInt160();
            Sha1(data, hash.Span);
            return hash;
        }

        public static void Sha256(this ReadOnlyByteSpan data, Span<byte> hash)
        {
            using var sha = new SHA256Managed();
            sha.TransformFinalBlock(data, hash);
        }

        public static UInt256 Sha256(this ReadOnlyByteSpan data)
        {
            var hash = new UInt256();
            Sha256(data, hash.Span);
            return hash;
        }

        public static void Sha512(this ReadOnlyByteSpan data, Span<byte> hash)
        {
            using var sha = new SHA512Managed();
            sha.TransformFinalBlock(data, hash);
        }

        public static UInt512 Sha512(this ReadOnlyByteSpan data)
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
        public static void Hash256(this ReadOnlyByteSpan data, ByteSpan hash)
        {
            var x = new UInt256();
            using var sha = new SHA256Managed();
            TransformFinalBlock(sha, data, x.Span);
            TransformFinalBlock(sha, x.Span, hash);
        }

        /// <summary>
        /// Computes double SHA256 of data: SHA256(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <returns>SHA256 of SHA256 of data.</returns>
        public static UInt256 Hash256(this ReadOnlyByteSpan data)
        {
            var hash = new UInt256();
            data.Hash256(hash.Span);
            return hash;
        }

        /// <summary>
        /// Computes RIPEMD160 of SHA256 of data: RIPEMD160(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <param name="hash">Output: RIPEMD160 of SHA256 of data.</param>
        public static void Hash160(this ReadOnlyByteSpan data, ByteSpan hash)
        {
            var x = data.Sha256();
            Ripemd160(x.Span, hash);
        }

        /// <summary>
        /// Computes RIPEMD160 of SHA256 of data: RIPEMD160(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <returns>KzHash160 RIPEMD160 of SHA256 of data.</returns>
        public static UInt160 Hash160(this ReadOnlyByteSpan data)
        {
            return Ripemd160(Sha256(data).Span);
        }

        public static void HmacSha256(this ReadOnlyByteSpan key, ReadOnlyByteSpan data, ByteSpan hash)
        {
            new HMACSHA256(key).TransformFinalBlock(data, hash);
        }

        public static UInt256 HmacSha256(this ReadOnlyByteSpan key, ReadOnlyByteSpan data)
        {
            var hash = new UInt256();
            new HMACSHA256(key).TransformFinalBlock(data, hash.Span);
            return hash;
        }

        public static void HmacSha512(this ReadOnlyByteSpan key, ReadOnlyByteSpan data, ByteSpan hash)
        {
            new HMACSHA512(key).TransformFinalBlock(data, hash);
        }

        public static UInt512 HmacSha512(this ReadOnlyByteSpan key, ReadOnlyByteSpan data)
        {
            var hash = new UInt512();
            new HMACSHA512(key).TransformFinalBlock(data, hash.Span);
            return hash;
        }

        public static byte[] ComputeHash(this HashAlgorithm alg, ReadOnlyByteSpan buffer)
        {
            var hash = new byte[alg.HashSize];
            alg.TransformFinalBlock(buffer, hash);
            return hash;
        }

        public static void TransformFinalBlock(this HashAlgorithm alg, ReadOnlyByteSpan data, ByteSpan hash)
        {
            var length = data.Length;
            var array = ArrayPool<byte>.Shared.Rent(Math.Min(MaxBufferSize, length));
            try
            {
                    var mOff = 0;
                    do 
                    {
                        var mLen = Math.Min(array.Length, data.Length - mOff);
                        data.Slice(mOff, mLen).CopyTo(array);
                        mOff += mLen;
                        if (mOff < length) 
                        {
                            alg.TransformBlock(array, 0, mLen, null, 0);
                        }
                        else 
                        {
                            alg.TransformFinalBlock(array, 0, mLen);
                            alg.Hash.CopyTo(hash);
                        }
                    } 
                    while (mOff < length);
            }
            finally 
            {
                Array.Clear(array, 0, array.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static void TransformBlock(this HashAlgorithm alg, ReadOnlyByteSpan data)
        {
            var length = data.Length;
            var array = ArrayPool<byte>.Shared.Rent(Math.Min(MaxBufferSize, length));
            try
            {
                var mOff = 0;
                do 
                {
                    var mLen = Math.Min(array.Length, data.Length - mOff);
                    data.Slice(mOff, mLen).CopyTo(array);
                    mOff += mLen;
                    alg.TransformBlock(array, 0, mLen, null, 0);
                }
                while (mOff < length);
            }
            finally
            {
                Array.Clear(array, 0, array.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }
    }
}
