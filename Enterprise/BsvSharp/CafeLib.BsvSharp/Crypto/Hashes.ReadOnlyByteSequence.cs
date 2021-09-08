#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.BsvSharp.BouncyCastle.Hash;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Crypto
{
    public static partial class Hashes
    {
        public static void Ripemd160(this ReadOnlyByteSequence data, ByteSpan hash)
        {
            Ripemd.Ripemd160(data, hash);
        }

        public static UInt160 Ripemd160(this ReadOnlyByteSequence data)
        {
            var h = new UInt160();
            Ripemd160(data, h.Span);
            return h;
        }

        public static void Sha1(this ReadOnlyByteSequence data, ByteSpan hash)
        {
            new Sha1().ComputeHash(data.ToSpan()).CopyTo(hash);
        }

        public static UInt160 Sha1(this ReadOnlyByteSequence data)
        {
            var hash = new UInt160();
            data.Sha1(hash.Span);
            return hash;
        }

        public static void Sha256(this ReadOnlyByteSequence data, ByteSpan hash)
        {
            new Sha256().ComputeHash(data.ToSpan()).CopyTo(hash);
        }

        public static UInt256 Sha256(this ReadOnlyByteSequence data)
        {
            var hash = new UInt256();
            data.Sha256(hash.Span);
            return hash;
        }

        public static void Sha512(this ReadOnlyByteSequence data, ByteSpan hash)
        {
            new Sha512().ComputeHash(data.ToSpan()).CopyTo(hash);
        }

        public static UInt512 Sha512(this ReadOnlyByteSequence data)
        {
            var hash = new UInt512();
            data.Sha512(hash.Span);
            return hash;
        }

        /// <summary>
        /// Computes double SHA256 of data: SHA256(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <param name="hash">Output: SHA256 of SHA256 of data.</param>
        public static void Hash256(this ReadOnlyByteSequence data, ByteSpan hash)
        {
            var h1 = new UInt256();
            using var sha = new SHA256Managed();
            TransformFinalBlock(sha, data, h1.Span);
            TransformFinalBlock(sha, h1.Span, hash);
        }

        /// <summary>
        /// Computes double SHA256 of data: SHA256(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <returns>SHA256 of SHA256 of data.</returns>
        public static UInt256 Hash256(this ReadOnlyByteSequence data)
        {
            var h2 = new UInt256();
            data.Hash256(h2.Span);
            return h2;
        }

        /// <summary>
        /// Computes RIPEMD160 of SHA256 of data: RIPEMD160(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <param name="hash">Output: RIPEMD160 of SHA256 of data.</param>
        public static void Hash160(this ReadOnlyByteSequence data, ByteSpan hash)
        {
            var h = data.Sha256();
            Ripemd160(h.Span, hash);
        }

        /// <summary>
        /// Computes RIPEMD160 of SHA256 of data: RIPEMD160(SHA256(data))
        /// </summary>
        /// <param name="data">Input: bytes to be hashed.</param>
        /// <returns>KzHash160 RIPEMD160 of SHA256 of data.</returns>
        public static UInt160 Hash160(this ReadOnlyByteSequence data)
        {
            return Ripemd160(Sha256(data).Span);
        }

        public static void HmacSha256(this ReadOnlyByteSpan key, ReadOnlyByteSequence data, ByteSpan hash)
        {
            new HMACSHA256(key).TransformFinalBlock(data, hash);
        }

        public static UInt256 HmacSha256(this ReadOnlyByteSpan key, ReadOnlyByteSequence data)
        {
            var hash = new UInt256();
            new HMACSHA256(key).TransformFinalBlock(data, hash.Span);
            return hash;
        }

        public static void HmacSha512(this ReadOnlyByteSpan key, ReadOnlyByteSequence data, ByteSpan hash)
        {
            new HMACSHA512(key).TransformFinalBlock(data, hash);
        }

        public static UInt512 HmacSha512(this ReadOnlyByteSpan key, ReadOnlyByteSequence data)
        {
            var hash = new UInt512();
            new HMACSHA512(key).TransformFinalBlock(data, hash.Span);
            return hash;
        }

        public static byte[] ComputeHash(this HashAlgorithm alg, ReadOnlyByteSequence buffer)
        {
            var hash = new byte[alg.HashSize];
            alg.TransformFinalBlock(buffer, hash);
            return hash;
        }

        public static void TransformFinalBlock(this HashAlgorithm alg, ReadOnlyByteSequence data, ByteSpan hash)
        {
            var length = data.Length;
            var array = ArrayPool<byte>.Shared.Rent((int)Math.Min(MaxBufferSize, length));
            try
            {
                var offset = 0L;
                foreach (var m in data.Data)
                {
                    var mOff = 0;
                    do 
                    {
                        var mLen = Math.Min(array.Length, m.Length - mOff);
                        m.Span.Slice(mOff, mLen).CopyTo(array);
                        mOff += mLen;
                        offset += mLen;
                        if (offset < length) 
                        {
                            alg.TransformBlock(array, 0, mLen, null, 0);
                        } 
                        else 
                        {
                            alg.TransformFinalBlock(array, 0, mLen);
                            alg.Hash.CopyTo(hash);
                        }
                    } 
                    while (mOff < m.Length);
                }
            }
            finally {
                Array.Clear(array, 0, array.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static void TransformBlock(this HashAlgorithm alg, ReadOnlyByteSequence data)
        {
            var length = data.Length;
            var array = ArrayPool<byte>.Shared.Rent((int)Math.Min(MaxBufferSize, length));
            try
            {
                foreach (var m in data) 
                {
                    var mOff = 0;
                    do {
                        var mLen = Math.Min(array.Length, m.Length - mOff);
                        m.Data.Span.Slice(mOff, mLen).CopyTo(array);
                        mOff += mLen;
                        alg.TransformBlock(array, 0, mLen, null, 0);
                    } while (mOff < m.Length);
                }
            }
            finally 
            {
                Array.Clear(array, 0, array.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }
    }
}
