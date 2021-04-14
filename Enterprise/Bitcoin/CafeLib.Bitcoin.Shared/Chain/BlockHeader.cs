#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;

namespace CafeLib.Bitcoin.Shared.Chain
{

    /// <summary>
    /// Closely mirrors the data and layout of a serialized Bitcoin block header.
    /// Focus is on efficiency when processing large blocks.
    /// Not intended to facilitate making dynamic changes to a block header (mining).
    /// Includes the following meta data in addition to standard Bitcoin block header data:
    /// <list type="table">
    /// <item><term>Height</term><description>The chain height associated with this block.</description></item>
    /// </list>
    /// </summary>
    public class BlockHeader
    {
        public const int BlockHeaderSize = 80;

        /// The following fields are computed or external, not essential.
        public DateTime TimeWhen => DateTime.UnixEpoch + TimeSpan.FromSeconds(Time);

        public UInt256 Hash { get; }

        public int Height { get; set; }

        /// Public access to essential header fields.

        public int Version { get; private set; }

        public UInt256 HashPrevBlock { get; private set; }

        public UInt256 HashMerkleRoot { get; private set; }
        public UInt32 Time { get; private set; }

        public UInt32 Bits { get; private set; }
    
        public UInt32 Nonce { get; private set; }

        public BlockHeader(UInt256 hash)
        {
            Hash = hash;
        }

        public BlockHeader
        (
            int version,
            UInt256 hashPrevBlock,
            UInt256 hashMerkleRoot,
            UInt32 time,
            UInt32 bits,
            UInt32 nonce, 
            UInt256 hash
        )
            : this(hash)
        {
            Version = version;
            HashPrevBlock = hashPrevBlock;
            HashMerkleRoot = hashMerkleRoot;
            Time = time;
            Bits = bits;
            Nonce = nonce;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ros"></param>
        /// <returns></returns>
        public bool TryReadBlockHeader(ref ReadOnlySequence<byte> ros)
        {
            var r = new SequenceReader<byte>(ros);
            if (!TryReadBlockHeader(ref r)) return false;

            ros = ros.Slice(r.Consumed);
            return true;
        }

        public bool TryReadBlockHeader(ref SequenceReader<byte> r)
        {
            if (r.Remaining < BlockHeaderSize)
                return false;

            var start = r.Position;

            var hashPrevBlock = HashPrevBlock;
            var hashMerkleRoot = HashMerkleRoot;

            if (!r.TryReadLittleEndian(out int version)) return false;
            if (!r.TryCopyToA(ref hashPrevBlock)) return false;
            if (!r.TryCopyToA(ref hashMerkleRoot)) return false;
            if (!r.TryReadLittleEndian(out uint time)) return false;
            if (!r.TryReadLittleEndian(out uint bits)) return false;
            if (!r.TryReadLittleEndian(out uint nonce)) return false;

            Version = version;
            HashPrevBlock = hashPrevBlock;
            HashMerkleRoot = hashMerkleRoot;
            Time = time;
            Bits = bits;
            Nonce = nonce;

            var end = r.Position;

            var blockBytes = r.Sequence.Slice(start, end).ToArray();
            using var sha256 = SHA256.Create();
            var hash1 = sha256.ComputeHash(blockBytes);
            var hash2 = sha256.ComputeHash(hash1);
            hash2.CopyTo(Hash.Bytes);
            return true;
        }
    }
}

