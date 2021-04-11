#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;

namespace CafeLib.Bitcoin.Shared.Crypto
{
    public static partial class Hashes
    {
        private const int MaxBufferSize = 1 << 20; // Max ArrayPool<byte>.Shared buffer size.

        public static void GetHashFinal(this HashAlgorithm alg, Span<byte> hash)
        {
            var data = new byte[0];
            alg.TransformFinalBlock(data, 0, 0);
            alg.Hash.CopyTo(hash);
        }

        public static byte[] GetHashFinal(this HashAlgorithm alg)
        {
            var data = new byte[0];
            alg.TransformFinalBlock(data, 0, 0);
            return alg.Hash;
        }

        public static void TransformFinalBlock(this HashAlgorithm alg, byte[] data, int start, int length, Span<byte> hash)
        {
            alg.TransformFinalBlock(data, start, length);
            alg.Hash.CopyTo(hash);
        }

        /// <summary>
        /// Hash used to implement BIP 32 key derivations.
        /// </summary>
        /// <param name="chainCode"></param>
        /// <param name="nChild"></param>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <param name="output">512 bit, 64 byte hash.</param>
        public static void Bip32Hash(UInt256 chainCode, uint nChild, byte header, ReadOnlyByteSpan data, ByteSpan output)
        {
            var len = data.Length;
            var buf = new byte[1 + len + 4]; // header, data, nChild
            var s = buf.AsSpan();
            s[0] = header;
            data.CopyTo(s.Slice(1, len));
            var num = s.Slice(1 + len, 4);
            num[0] = (byte)((nChild >> 24) & 0xFF);
            num[1] = (byte)((nChild >> 16) & 0xFF);
            num[2] = (byte)((nChild >> 8) & 0xFF);
            // ReSharper disable once ShiftExpressionRealShiftCountIsZero
            num[3] = (byte)((nChild >> 0) & 0xFF);

            HmacSha512(chainCode.Bytes, s, output);
        }

        /// <summary>
        /// Duplicates Python hash library pbkdf2_hmac for hash_name = 'sha512' and dklen = None
        ///
        /// Performance can be improved by pre-computing _trans_36 and _trans_5c.
        /// Unlike Python's hash functions, .NET doesn't currently support copying state between blocks.
        /// This results in having to recompute hash of innerSeed and outerSeed on each iteration.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static UInt512 PbKdf2HmacSha512(ReadOnlyByteSpan password, ReadOnlyByteSpan salt, int iterations)
        {
            if (iterations < 1)
                throw new ArgumentException();

            byte[] passwordBytes = password;

            using var inner = new SHA512Managed();
            using var outer = new SHA512Managed();

            const int blocksize = 128; // match python hash library sha512 block size.

            if (passwordBytes.Length > blocksize)
            {
                inner.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
                passwordBytes = inner.Hash;
                //inner.Initialize();
            }

            if (passwordBytes.Length < blocksize)
                Array.Resize(ref passwordBytes, blocksize);

            var trans36 = new byte[256];
            var trans5C = new byte[256];
            for (var i = 0; i < 256; i++)
            {
                trans36[i] = (byte)(i ^ 0x36);
                trans5C[i] = (byte)(i ^ 0x5c);
            }

            var innerSeed = passwordBytes.Select(pb => trans36[pb]).ToArray();
            var outerSeed = passwordBytes.Select(pb => trans5C[pb]).ToArray();

            var hash = new UInt512();
            var xhash = new UInt512();

            var data = new byte[salt.Length + 4];
            salt.CopyTo(data);
            var loop = 1;
            loop.AsReadOnlySpan(bigEndian: true).CopyTo(data.AsSpan(salt.Length));
            var dataSpan = data.AsSpan();

            for (var i = 0; i < iterations; i++)
            {
                inner.TransformBlock(innerSeed);
                outer.TransformBlock(outerSeed);
                inner.TransformFinalBlock(dataSpan, hash.Bytes);
                outer.TransformFinalBlock(hash.Bytes, hash.Bytes);
                dataSpan = hash.Bytes;
                xhash = i == 0 ? hash : xhash ^ hash;
            }

            return xhash;
        }
    }
}
