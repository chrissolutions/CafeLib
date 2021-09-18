#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using CafeLib.Core.Buffers;
using CafeLib.Core.Numerics;
using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;

namespace CafeLib.Cryptography
{
    public class Ripemd
    {
        public static UInt160 Ripemd160(ReadOnlySpan<byte> data)
        {
            var h = new UInt160();
            Ripemd160(data, h.Span);
            return h;
        }

        public static UInt160 Ripemd160(ReadOnlySequence<byte> data)
        {
            var h = new UInt160();
            Ripemd160(data, h.Span);
            return h;
        }

        public static void Ripemd160(ReadOnlyByteSpan data, ByteSpan hash)
        {
            var bytes = new byte[UInt160.Length];
            var d = new RipeMD160Digest();
            d.BlockUpdate(data, 0, data.Length);
            d.DoFinal(bytes, 0);
            bytes.CopyTo(hash);
        }

        public static void Ripemd160(ReadOnlyByteSequence data, ByteSpan hash)
        {
            var d = new RipeMD160Digest();
            foreach (var m in data) {
                d.BlockUpdate(m, 0, m.Length);
            }
            d.DoFinal(hash, 0);
        }

        public static byte[] Ripemd160(byte[] data, int offset, int count)
        {
            var d = new RipeMD160Digest();
            d.BlockUpdate(data, offset, count);
            var rv = new byte[20];
            d.DoFinal(rv, 0);
            return rv;
        }
    }
}
