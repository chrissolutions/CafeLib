#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Chain
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

        /// Essential fields of a Bitcoin SV block header.
        private int _version;
        private UInt256 _hashPrevBlock;
        private UInt256 _hashMerkleRoot;
        private uint _timestamp;
        private uint _bits;
        private uint _nonce;


        /// The following fields are computed or external, not essential.
        public DateTime TimeWhen => DateTime.UnixEpoch + TimeSpan.FromSeconds(_timestamp);

        private readonly UInt256 _hash = new UInt256();
        public UInt256 Hash => _hash;

        public int Height { get; set; }

        /// Public access to essential header fields.
        public int Version => _version;
        public UInt256 HashPrevBlock => _hashPrevBlock;
        public UInt256 HashMerkleRoot => _hashMerkleRoot;
        public uint Timestamp => _timestamp;
        public uint Bits => _bits;
        public uint Nonce => _nonce;

        public BlockHeader()
        {
        }

        public BlockHeader
        (
            int version,
            UInt256 hashPrevBlock,
            UInt256 hashMerkleRoot,
            uint timestamp,
            uint bits,
            uint nonce
        )
        {
            _version = version;
            _hashPrevBlock = hashPrevBlock;
            _hashMerkleRoot = hashMerkleRoot;
            _timestamp = timestamp;
            _bits = bits;
            _nonce = nonce;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ros"></param>
        /// <returns></returns>
        public bool TryReadBlockHeader(ref ReadOnlyByteSequence ros)
        {
            var r = new ByteSequenceReader(ros);
            if (!TryReadBlockHeader(ref r)) return false;
            ros = ros.Data.Slice(r.Data.Consumed);
            return true;
        }

        public bool TryReadBlockHeader(ref ByteSequenceReader r)
        {
            if (r.Data.Remaining < BlockHeaderSize)
                return false;

            var start = r.Data.Position;

            if (!r.TryReadLittleEndian(out _version)) return false;
            if (!r.TryReadUInt256(ref _hashPrevBlock)) return false;
            if (!r.TryReadUInt256(ref _hashMerkleRoot)) return false;
            if (!r.TryReadLittleEndian(out _timestamp)) return false;
            if (!r.TryReadLittleEndian(out _bits)) return false;
            if (!r.TryReadLittleEndian(out _nonce)) return false;

            var end = r.Data.Position;

            var blockBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using var sha256 = SHA256.Create();
            var hash1 = sha256.ComputeHash(blockBytes);
            var hash2 = sha256.ComputeHash(hash1);
            hash2.CopyTo(_hash.Span);
            return true;
        }
    }
}
