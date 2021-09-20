using System;
using CafeLib.Core.Buffers;
using CafeLib.Core.Extensions;
using CafeLib.Core.Numerics;
using CafeLib.Cryptography.BouncyCastle.Hash;

namespace CafeLib.Cryptography
{
    public static class Hashes
    {
        public static UInt160 Ripemd160(this ReadOnlyByteSpan data)
        {
            var hash = new UInt160();
            Ripemd.Ripemd160(data, hash.Span);
            return hash;
        }

        public static void Sha1(this ReadOnlyByteSpan data, ByteSpan hash)
        {
            var computed = ComputeSha1(data);
            computed.CopyTo(hash);
        }

        public static UInt160 Sha1(this ReadOnlyByteSpan data)
        {
            var hash = new UInt160();
            Sha1(data, hash.Span);
            return hash;
        }

        public static void Sha256(this ReadOnlyByteSpan data, ByteSpan hash)
        {
            var computed = ComputeSha256(data);
            computed.CopyTo(hash);
        }

        public static UInt256 Sha256(this ReadOnlyByteSpan data)
        {
            var hash = new UInt256();
            Sha256(data, hash.Span);
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

        public static void Sha512(this ReadOnlyByteSpan data, ByteSpan hash)
        {
            var computed = ComputeSha512(data);
            computed.CopyTo(hash);
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
            var computed = ComputeSha256(ComputeSha256(data));
            computed.CopyTo(hash);
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
        /// <returns>KzHash160 RIPEMD160 of SHA256 of data.</returns>
        public static UInt160 Hash160(this ReadOnlyByteSpan data)
        {
            return Ripemd160(Sha256(data).Span);
        }

        public static void HmacSha256(this ReadOnlyByteSpan key, ReadOnlyByteSpan data, ByteSpan hash)
        {
            new HmacSha256(key).ComputeHash(data).CopyTo(hash);
        }

        public static UInt256 HmacSha256(this ReadOnlyByteSpan key, ReadOnlyByteSpan data)
        {
            var hash = new UInt256();
            HmacSha256(key, data, hash.Span);
            return hash;
        }

        public static void HmacSha512(this ReadOnlyByteSpan key, ReadOnlyByteSpan data, ByteSpan hash)
        {
            new HmacSha512(key).ComputeHash(data).CopyTo(hash);
        }

        public static UInt512 HmacSha512(this ReadOnlyByteSpan key, ReadOnlyByteSpan data)
        {
            var hash = new UInt512();
            HmacSha512(key, data, hash.Span);
            return hash;
        }

        public static byte[] ComputeSha1(byte[] data) => new Sha1().ComputeHash(data);

        public static byte[] ComputeSha256(byte[] data) => new Sha256().ComputeHash(data);

        public static byte[] ComputeSha512(byte[] data) => new Sha512().ComputeHash(data);

        /// <summary>
        /// Obtain BIP32 hash
        /// </summary>
        /// <param name="chainCode"></param>
        /// <param name="nChild"></param>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <returns>512 bit, 64 byte hash</returns>
        public static byte[] Bip32Hash(byte[] chainCode, uint nChild, byte header, byte[] data)
        {
            var num = new byte[4];
            num[0] = (byte)((nChild >> 24) & 0xFF);
            num[1] = (byte)((nChild >> 16) & 0xFF);
            num[2] = (byte)((nChild >> 8) & 0xFF);
            num[3] = (byte)(nChild & 0xFF);

            return HmacSha512(chainCode,
                new[] { header }
                .Concat(data)
                .Concat(num));
        }
    }
}
