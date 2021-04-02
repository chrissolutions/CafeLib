﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Utility;

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
    public class KzBlockHeader
    {
        public const int BlockHeaderSize = 80;

        /// Essential fields of a Bitcoin SV block header.

        Int32 _version;
        KzUInt256 _hashPrevBlock;
        KzUInt256 _hashMerkleRoot;
        UInt32 _time;
        UInt32 _bits;
        UInt32 _nonce;

        /// The following fields are computed or external, not essential.

        public DateTime TimeWhen => DateTime.UnixEpoch + TimeSpan.FromSeconds(_time);

        KzUInt256 _hash;
        public KzUInt256 Hash => _hash;

        public Int32 Height { get; set; }

        /// Public access to essential header fields.

        public Int32 Version => _version;
        public KzUInt256 HashPrevBlock => _hashPrevBlock;
        public KzUInt256 HashMerkleRoot => _hashMerkleRoot;
        public UInt32 Time => _time;
        public UInt32 Bits => _bits;
        public UInt32 Nonce => _nonce;

        public KzBlockHeader() { }

        public KzBlockHeader(
            Int32 version,
            KzUInt256 hashPrevBlock,
            KzUInt256 hashMerkleRoot,
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
        public bool TryReadBlockHeader(ref ReadOnlySequence<byte> ros)
        {
            var r = new SequenceReader<byte>(ros);
            if (!TryReadBlockHeader(ref r)) goto fail;

            ros = ros.Slice(r.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadBlockHeader(ref SequenceReader<byte> r)
        {
            if (r.Remaining < BlockHeaderSize)
                return false;

            var start = r.Position;

            if (!r.TryReadLittleEndian(out _version)) goto fail;
            if (!r.TryCopyToA(ref _hashPrevBlock)) goto fail;
            if (!r.TryCopyToA(ref _hashMerkleRoot)) goto fail;
            if (!r.TryReadLittleEndian(out _time)) goto fail;
            if (!r.TryReadLittleEndian(out _bits)) goto fail;
            if (!r.TryReadLittleEndian(out _nonce)) goto fail;

            var end = r.Position;

            var blockBytes = r.Sequence.Slice(start, end).ToArray();
            using (var sha256 = SHA256.Create())
            {
                var hash1 = sha256.ComputeHash(blockBytes);
                var hash2 = sha256.ComputeHash(hash1);
                hash2.CopyTo(_hash.Span);
            }

            return true;
        fail:
            return false;
        }
    }
}

