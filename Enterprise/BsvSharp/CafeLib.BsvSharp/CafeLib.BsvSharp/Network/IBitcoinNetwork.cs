#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Buffers;

namespace CafeLib.BsvSharp.Network
{
    public interface IBitcoinNetwork
    {
        Consensus Consensus { get; }

        string NetworkId { get; }

        /// <summary>
        /// Base58 encoding prefix for public key addresses for the active network.
        /// </summary>
        ReadOnlyByteSpan PublicKeyAddress { get; }

        /// <summary>
        /// Base58 encoding prefix for script addresses for the active network.
        /// </summary>
        ReadOnlyByteSpan ScriptAddress { get; }

        /// <summary>
        /// Base58 encoding prefix for private keys for the active network.
        /// </summary>
        ReadOnlyByteSpan SecretKey { get; }

        /// <summary>
        /// Base58 encoding prefix for extended public keys for the active network.
        /// </summary>
        ReadOnlyByteSpan ExtPublicKey { get; }

        /// <summary>
        /// Base58 encoding prefix for extended private keys for the active network.
        /// </summary>
        ReadOnlyByteSpan ExtSecretKey { get; }
    }
}
