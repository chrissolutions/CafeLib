#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;

namespace CafeLib.Bitcoin.Chain
{

    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// In particular, script data is stored as <see cref="ReadOnlySequence{Byte}"/> allowing large scripts to
    /// remain in whatever buffers were originally used. No script parsing data is maintained. 
    /// Not intended for making dynamic changes to a transaction (adding inputs, outputs, signing).
    /// See <see cref="KzBTransaction"/> when dynamically building a transaction to send.
    /// In addition, transactions associated with specific wallets should consider KzWallets.KzWdbTx for transaction data.
    /// Includes the following pre-computed meta data in addition to standard Bitcoin transaction data:
    /// <list type="table">
    /// <item><term>HashTx</term><description>Transaction's hash.</description></item>
    /// </list>
    /// </summary>
    public class Transaction
    {
        /// Public access to essential header fields.

        public int Version { get; set; }

        public UInt32 LockTime { get; set; }

        public TxIn[] Inputs { get; set; } = new TxIn[0];

        public TxOut[] Outputs { get; set; } = new TxOut[0];

        /// Public access to computed or external, not essential.

        public UInt256 Hash { get; }

        public Transaction()
        {
        }

        public Transaction(Int32 version, TxIn[] vin, TxOut[] vout, UInt32 lockTime)
        {
            Version = version;
            Inputs = vin;
            Outputs = vout;
            LockTime = lockTime;
        }

        //public Transaction(KzBTransaction tb)
        //{
        //    _version = tb.Version;
        //    _vin = tb.Vin.Select(i => i.ToTxIn()).ToArray();
        //    _vout = tb.Vout.Select(o => o.ToTxOut()).ToArray();
        //    _lockTime = tb.LockTime;
        //}

        public bool TryReadTransaction(ref ReadOnlyByteSequence ros)
        {
            var r = new ByteSequenceReader(ros);
            if (!TryReadTransaction(ref r)) return false;
            ros = ros.Data.Slice(r.Data.Consumed);
            return true;
        }

        public bool TryParseTransaction(ref ByteSequenceReader r, IBlockParser bp)
        {
            var offset = r.Data.Consumed;
            var start = r.Data.Position;

            if (!r.TryReadLittleEndian(out int version)) goto fail;
            if (!r.TryReadVarInt(out var countIn)) goto fail;

            bp.TxStart(this, offset);

            var vin = new TxIn[countIn];
            for (var i = 0L; i < countIn; i++)
            {
                ref var txIn = ref vin[i];
                if (!txIn.TryParseTxIn(ref r, bp)) goto fail;
            }

            if (!r.TryReadVarInt(out var countOut)) goto fail;

            var vout = new TxOut[countOut];
            for (var i = 0L; i < countOut; i++)
            {
                ref var txOut = ref vout[i];
                if (!txOut.TryParseTxOut(ref r, bp)) goto fail;
            }

            if (!r.TryReadLittleEndian(out uint lockTime)) goto fail;

            var end = r.Data.Position;

            // Compute the transaction hash.
            var txBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using (var sha256 = SHA256.Create())
            {
                var hash1 = sha256.ComputeHash(txBytes);
                var hash2 = sha256.ComputeHash(hash1);
                hash2.CopyTo(Hash.Bytes);
            }

            bp.TxParsed(this, r.Data.Consumed);

            Version = version;
            Inputs = vin;
            Outputs = vout;
            LockTime = lockTime;
            return true;

            fail:
            return false;
        }

        public bool TryReadTransaction(ref ByteSequenceReader r)
        {
            var start = r.Data.Position;

            if (!r.TryReadLittleEndian(out int version)) return false;
            if (!r.TryReadVarInt(out var countIn)) return false;

            var vin = new TxIn[countIn];
            for (var i = 0L; i < countIn; i++)
            {
                ref var txIn = ref Inputs[i];
                if (!txIn.TryReadTxIn(ref r)) return false;
            }

            if (!r.TryReadVarInt(out var countOut)) return false;

            var vout = new TxOut[countOut];
            for (var i = 0L; i < countOut; i++)
            {
                ref var txOut = ref Outputs[i];
                if (!txOut.TryReadTxOut(ref r)) return false;
            }

            if (!r.TryReadLittleEndian(out uint lockTime)) return false;

            var end = r.Data.Position;

            // Compute the transaction hash.
            var txBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using var sha256 = SHA256.Create();
            var hash1 = sha256.ComputeHash(txBytes);
            var hash2 = sha256.ComputeHash(hash1);
            hash2.CopyTo(Hash.Bytes);
            Version = version;
            Inputs = vin;
            Outputs = vout;
            LockTime = lockTime;
            return true;
        }

        public static Transaction ParseHex(string rawTxHex)
        {
            var bytes = rawTxHex.HexToBytes();
            var tx = new Transaction();
            var ros = new ReadOnlyByteSequence(bytes);
            if (!tx.TryReadTransaction(ref ros)) tx = null;
            return tx;
        }

        public override string ToString()
        {
            return Hash.ToString();
        }

        public IBitcoinWriter AddTo(IBitcoinWriter writer)
        {
            writer
                .Add(Version)
                .Add(Inputs.Length.AsVarIntBytes())
                ;

            foreach (var txIn in Inputs)
                //writer
                //.Add(txIn)
                ;
            writer
                .Add(Outputs.Length.AsVarIntBytes())
                ;
            foreach (var txOut in Outputs)
                //writer
                //    .Add(txOut)
                    ;
            writer
                .Add(LockTime)
                ;
            return writer;
        }

        public byte[] ToBytes()
        {
            var wl = new WriterLength();
            wl.Add(this);
            var length = wl.Length;
            var bytes = new byte[length];
            var wm = new MemoryWriter(new Memory<byte>(bytes));
            wm.Add(this);
            return bytes;
        }
    }
}
