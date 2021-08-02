#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.BsvSharp.Crypto;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Numerics;
using CafeLib.Core.Buffers;

namespace CafeLib.BsvSharp.Persistence
{
    public class HashWriter : IDisposable, IBitcoinWriter, IDataWriter
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

        public IBitcoinWriter Add(byte[] data) { _alg.TransformBlock(data); return this; }
        public IBitcoinWriter Add(ReadOnlySpan<byte> data) { _alg.TransformBlock(data); return this; }
        public IBitcoinWriter Add(ReadOnlySequence<byte> data) { _alg.TransformBlock(data); return this; }

        public IBitcoinWriter Add(UInt64 v) { _alg.TransformBlock(v.AsReadOnlySpan()); return this; }
        public IBitcoinWriter Add(Int64 v) { _alg.TransformBlock(v.AsReadOnlySpan()); return this; }
        public IBitcoinWriter Add(UInt32 v) { _alg.TransformBlock(v.AsReadOnlySpan()); return this; } 
        public IBitcoinWriter Add(Int32 v) { _alg.TransformBlock(v.AsReadOnlySpan()); return this; }
        public IBitcoinWriter Add(ReadOnlyByteSpan data)
        {
            _alg.TransformBlock(data.ToArray());
            return this;
        }

        public IBitcoinWriter Add(ReadOnlyByteSequence data)
        {
            _alg.TransformBlock(data);
            return this;
        }

        public IBitcoinWriter Add(byte v)
        {
            _alg.TransformBlock(new[] { v });
            return this;
        }

        public IBitcoinWriter Add(UInt160 v)
        {
            _alg.TransformBlock(v.Span); 
            return this;
        }

        public IBitcoinWriter Add(UInt256 v)
        {
            _alg.TransformBlock(v.Span);
            return this;
        }

        public IBitcoinWriter Add(UInt512 v)
        {
            _alg.TransformBlock(v.Span); 
            return this;
        } 

        public HashWriter Add(string ascii)
        {
            _alg.TransformBlock(((VarInt)ascii.Length).ToArray());
            _alg.TransformBlock(ascii.AsciiToBytes());
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
            data.AsReadOnlySpan();
            return this;
        }

        public IDataWriter Write(uint data)
        {
            data.AsReadOnlySpan();
            return this;
        }

        public IDataWriter Write(long data)
        {
            data.AsReadOnlySpan();
            return this;
        }

        public IDataWriter Write(ulong data)
        {
            data.AsReadOnlySpan();
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
