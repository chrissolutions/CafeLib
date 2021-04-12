#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Extensions;

namespace CafeLib.Bitcoin.Shared.Network
{
    public abstract class BitcoinNetwork : IBitcoinNetwork
    {
        protected static readonly object Mutex = new object();

        //public KzConsensus Consensus { get; protected set; }

        public string NetworkId { get; }

        protected byte[][] Base58Prefixes { get; }

        protected BitcoinNetwork(NetworkType nodeType, byte[][] base58Prefixes)
        {
            NetworkId = nodeType.GetDescriptor();
            Base58Prefixes = base58Prefixes;
        }

        public ReadOnlyByteSpan PublicKeyAddress => new Lazy<byte[]>(() => CreateKey(Base58Type.PubkeyAddress)).Value;

        public ReadOnlyByteSpan ScriptAddress => new Lazy<byte[]>(() => CreateKey(Base58Type.ScriptAddress)).Value;

        public ReadOnlyByteSpan SecretKey => new Lazy<byte[]>(() => CreateKey(Base58Type.SecretKey)).Value;

        public ReadOnlyByteSpan ExtPublicKey => new Lazy<byte[]>(() => CreateKey(Base58Type.ExtPublicKey)).Value;

        public ReadOnlyByteSpan ExtSecretKey => new Lazy<byte[]>(() => CreateKey(Base58Type.ExtSecretKey)).Value;

        private ReadOnlyByteSpan Base58Prefix(Base58Type type) => Base58Prefixes[(int)type].AsSpan();

        private ReadOnlyByteSpan CreateKey(Base58Type networkType)
        {
            lock (Mutex)
            {
                return Base58Prefix(networkType);
            }
        }
    }
}
