#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using CafeLib.Bitcoin.Utility;

namespace CafeLib.Bitcoin.Shared.Crypto
{
    public class Ripemd
    {
        public static KzUInt160 Ripemd160(ReadOnlySpan<byte> data)
        {
            var h = new KzUInt160();
            Ripemd160(data, h.Span);
            return h;
        }

        public static KzUInt160 Ripemd160(ReadOnlySequence<byte> data)
        {
            var h = new KzUInt160();
            Ripemd160(data, h.Span);
            return h;
        }

        public static void Ripemd160(ReadOnlySpan<byte> data, Span<byte> hash)
        {
            var d = new Ripemd160Digest();
            d.BlockUpdate(data);
            d.DoFinal(hash);
        }

        public static void Ripemd160(ReadOnlySequence<byte> data, Span<byte> hash)
        {
            var d = new Ripemd160Digest();
            foreach (var m in data) {
                d.BlockUpdate(m.Span);
            }
            d.DoFinal(hash);
        }

        public static byte[] Ripemd160(byte[] data, int offset, int count)
        {
            var d = new Ripemd160Digest();
            d.BlockUpdate(data.AsSpan(offset, count));
            var rv = new byte[20];
            d.DoFinal(rv.AsSpan<byte>());
            return rv;
        }
    }
}
