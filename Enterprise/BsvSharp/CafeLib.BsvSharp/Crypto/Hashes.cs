#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;
using CafeLib.Cryptography.BouncyCastle.Crypto.Macs;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;
using CafeLib.Cryptography.BouncyCastle.Hash;
using CafeLib.BsvSharp.Crypto.Cryptsharp;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Crypto
{
    public static partial class Hashes
    {
        private const int MaxBufferSize = 1 << 20; // Max ArrayPool<byte>.Shared buffer size.

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

            HmacSha512(chainCode.Span, s, output);
        }

        public static UInt512 Bip39Seed(string mnemonic, string passphrase = null, string passwordPrefix = "mnemonic")
        {
            var salt = $"{passwordPrefix}{passphrase}".Utf8NormalizedToBytes();
            var bytes = mnemonic.Utf8NormalizedToBytes();

            var mac = new HMac(new Sha512Digest());
            mac.Init(new KeyParameter(bytes));
            var key = Pbkdf2.ComputeDerivedKey(mac, salt, 2048, 64);
            return new UInt512(key);
        }

        public static byte[] ComputeSha1(byte[] data) => new Sha1().ComputeHash(data);

        public static byte[] ComputeSha256(byte[] data) => new Sha256().ComputeHash(data);

        public static byte[] ComputeSha512(byte[] data) => new Sha512().ComputeHash(data);
    }
}
