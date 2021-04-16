#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Collections.Generic;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Scripting;

namespace CafeLib.Bitcoin.Shared.Chain
{

    /// <summary>
    /// Closely mirrors the data and layout of a serialized Bitcoin block.
    /// Focus is on efficiency when processing large blocks.
    /// In particular, script data is stored as <see cref="ReadOnlySequence{Byte}"/> allowing large scripts to
    /// remain in whatever buffers were originally used. No script parsing data is maintained. 
    /// Not intended for making dynamic changes to a block (mining).
    /// </summary>
    public class Block : BlockHeader
    {
        public Transaction[] Txs { get; private set; }

        public Block()
        {
        }

        public Block
        (
            Transaction[] txs,
            Int32 version,
            UInt256 hashPrevBlock,
            UInt256 hashMerkleRoot,
            UInt32 time,
            UInt32 bits,
            UInt32 nonce
        )
            : base(version, hashPrevBlock, hashMerkleRoot, time, bits, nonce)
        {
            Txs = txs;
        }

        public bool TryParseBlock(ref ReadOnlySequence<byte> ros, int height, IBlockParser bp)
        {
            var r = new SequenceReader<byte>(ros);
            if (!TryParseBlock(ref r, height, bp)) goto fail;

            ros = ros.Slice(r.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadBlock(ref ReadOnlySequence<byte> ros)
        {
            var r = new SequenceReader<byte>(ros);
            if (!TryReadBlock(ref r)) goto fail;

            ros = ros.Slice(r.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryParseBlock(ref ByteSequenceReader r, int height, IBlockParser bp)
        {
            var offset = r.Data.Consumed;

            if (!TryReadBlockHeader(ref r)) goto fail;

            Height = height;

            bp.BlockStart(this, offset);

            if (!r.TryReadVarInt(out var count)) goto fail;

            Txs = new Transaction[count];

            for (var i = 0L; i < count; i++)
            {
                var t = new Transaction();
                Txs[i] = t;
                if (!t.TryParseTransaction(ref r, bp)) goto fail;
            }

            if (!VerifyMerkleRoot()) goto fail;

            bp.BlockParsed(this, r.Data.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadBlock(ref ByteSequenceReader r)
        {
            if (!TryReadBlockHeader(ref r)) goto fail;

            if (!r.TryReadVarInt(out var count)) goto fail;

            Txs = new Transaction[count];

            for (var i = 0L; i < count; i++)
            {
                var t = new Transaction();
                Txs[i] = t;
                if (!t.TryReadTransaction(ref r)) goto fail;
            }

            if (!VerifyMerkleRoot()) goto fail;

            return true;
        fail:
            return false;
        }

        private UInt256 ComputeMerkleRoot() => MerkleTree.ComputeMerkleRoot(Txs);

        bool VerifyMerkleRoot() => ComputeMerkleRoot() == HashMerkleRoot;

        public IEnumerable<(Transaction tx, TxOut o, int i)> GetOutputsSendingToAddresses(UInt160[] addresses)
        {
            var v = new UInt160();
            foreach (var tx in Txs) {
                foreach (var o in tx.Vout) {
                    foreach (var op in o.ScriptPub.Decode()) {
                        if (op.Code == Opcode.OP_PUSH20) {
                            op.Data.ToSpan().CopyTo(v.Bytes);
                            var i = Array.BinarySearch(addresses, v);
                            if (i >= 0) {
                                yield return (tx, o, i);
                            }
                        }
                    }
                }
            }
        }

    }
}
