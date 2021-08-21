#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Security.Cryptography;
using CafeLib.BsvSharp.Crypto;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Persistence
{
    public class HashWriter : IDisposable, IDataWriter
    {
        private readonly SHA256Managed _alg = new SHA256Managed();

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        protected virtual void Dispose(bool alsoCleanupManaged)
        {
            _alg.Dispose();
        }

        public UInt256 GetHashFinal()
        {
            var hash = _alg.GetHashFinal();
            _alg.TransformFinalBlock(hash, 0, hash.Length);
            hash = _alg.Hash;
            return hash.AsUInt256();
        }

        public IDataWriter Write(ReadOnlyByteSpan data)
        {
            _alg.TransformBlock(data);
            return this;
        }

        public IDataWriter Write(ReadOnlyByteSequence data)
        {
            _alg.TransformBlock(data);
            return this;
        }

        public IDataWriter Write(byte[] data)
        {
            _alg.TransformBlock(data);
            return this;
        }

        public IDataWriter Write(byte data)
        {
            _alg.TransformBlock(new[] { data });
            return this;
        }

        public IDataWriter Write(int data)
        {
            _alg.TransformBlock(data.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(uint data)
        {
            _alg.TransformBlock(data.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(long data)
        {
            _alg.TransformBlock(data.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(ulong data)
        {
            _alg.TransformBlock(data.AsReadOnlySpan());
            return this;
        }

        public IDataWriter Write(string data)
        {
            _alg.TransformBlock(((VarInt)data.Length).ToArray());
            _alg.TransformBlock(data.AsciiToBytes());
            return this;
        }

        public IDataWriter Write(UInt160 data)
        {
            _alg.TransformBlock(data.Span);
            return this;
        }

        public IDataWriter Write(UInt256 data)
        {
            _alg.TransformBlock(data.Span);
            return this;
        }

        public IDataWriter Write(UInt512 data)
        {
            _alg.TransformBlock(data.Span);
            return this;
        }
    }
}
