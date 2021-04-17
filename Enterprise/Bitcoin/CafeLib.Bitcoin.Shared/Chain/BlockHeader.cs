#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Shared.Buffers;
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

        /// Essential fields of a Bitcoin SV block header.

        Int32 _version;
        UInt256 _hashPrevBlock;
        UInt256 _hashMerkleRoot;
        UInt32 _time;
        UInt32 _bits;
        UInt32 _nonce;

        /// The following fields are computed or external, not essential.

        public DateTime TimeWhen => DateTime.UnixEpoch + TimeSpan.FromSeconds(_time);

        private UInt256 _hash;
        public UInt256 Hash => _hash;

        public Int32 Height { get; set; }

        /// Public access to essential header fields.

        public Int32 Version => _version;
        public UInt256 HashPrevBlock => _hashPrevBlock;
        public UInt256 HashMerkleRoot => _hashMerkleRoot;
        public UInt32 Time => _time;
        public UInt32 Bits => _bits;
        public UInt32 Nonce => _nonce;

        public BlockHeader()
        {
        }

        public BlockHeader
        (
            Int32 version,
            UInt256 hashPrevBlock,
            UInt256 hashMerkleRoot,
            UInt32 time,
            UInt32 bits,
            UInt32 nonce
        )
        {
            _version = version;
            _hashPrevBlock = hashPrevBlock;
            _hashMerkleRoot = hashMerkleRoot;
            _time = time;
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
            if (!r.TryReadLittleEndian(out _time)) return false;
            if (!r.TryReadLittleEndian(out _bits)) return false;
            if (!r.TryReadLittleEndian(out _nonce)) return false;

            var end = r.Data.Position;

            var blockBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using var sha256 = SHA256.Create();
            var hash1 = sha256.ComputeHash(blockBytes);
            var hash2 = sha256.ComputeHash(hash1);
            hash2.CopyTo(_hash.Bytes);
            return true;
        }
    }
}
