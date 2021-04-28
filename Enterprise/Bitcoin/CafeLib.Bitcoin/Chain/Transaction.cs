#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using System.Linq;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Builders;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;
using CafeLib.Bitcoin.Scripting;
using CafeLib.Bitcoin.Units;

namespace CafeLib.Bitcoin.Chain
{

    /// <summary>
    /// Closely mirrors the data and layout of a Bitcoin transaction as stored in each block.
    /// Focus is on performance when processing large numbers of transactions, including blocks of transactions.
    /// In particular, script data is stored as <see cref="ReadOnlySequence{Byte}"/> allowing large scripts to
    /// remain in whatever buffers were originally used. No script parsing data is maintained. 
    /// Not intended for making dynamic changes to a transaction (adding inputs, outputs, signing).
    /// See <see cref="TransactionBuilder"/> when dynamically building a transaction to send.
    /// In addition, transactions associated with specific wallets should consider KzWallets.KzWdbTx for transaction data.
    /// Includes the following pre-computed meta data in addition to standard Bitcoin transaction data:
    /// <list type="table">
    /// <item><term>HashTx</term><description>Transaction's hash.</description></item>
    /// </list>
    /// </summary>
    public class Transaction
    {
        /// Essential fields of a Bitcoin SV transaction.
        private int _version;
        private TxIn[] _vin = new TxIn[0];
        private TxOut[] _vout = new TxOut[0];
        private UInt32 _lockTime;

        /// The following fields are computed or external, not essential.
        private readonly UInt256 _txHash = new UInt256();
        //Int64 _valueIn;
        //Int64 _valueOut;

        /// Public access to essential header fields.
        public Int32 Version => _version;
        public uint LockTime => _lockTime;

        public TxIn[] Inputs => _vin;
        public TxOut[] Outputs => _vout;

        /// Public access to computed or external, not essential.

        public UInt256 TxId => _txHash;
        public UInt256 Hash => _txHash;

        public Transaction() { }

        public Transaction(int version, TxIn[] vin, TxOut[] vout, uint lockTime)
        {
            _version = version;
            _vin = vin;
            _vout = vout;
            _lockTime = lockTime;
        }

        public Transaction(TransactionBuilder tb)
        {
            _version = tb.Version;
            _vin = tb.Vin.Select(i => i.ToTxIn()).ToArray();
            _vout = tb.Vout.Select(o => o.ToTxOut()).ToArray();
            _lockTime = tb.LockTime;
        }

        public bool TryReadTransaction(ref ReadOnlyByteSequence ros)
        {
            var r = new ByteSequenceReader(ros);
            if (!TryReadTransaction(ref r)) goto fail;

            ros = ros.Data.Slice(r.Data.Consumed);

            return true;
            fail:
            return false;
        }

        public bool TryParseTransaction(ref ByteSequenceReader r, IBlockParser bp)
        {
            var offset = r.Data.Consumed;
            var start = r.Data.Position;

            if (!r.TryReadLittleEndian(out _version)) return false;
            if (!r.TryReadVariant(out var countIn)) return false;

            bp.TxStart(this, offset);

            _vin = new TxIn[countIn];
            for (var i = 0L; i < countIn; i++)
            {
                ref var txIn = ref _vin[i];
                if (!txIn.TryParseTxIn(ref r, bp)) return false;
            }

            if (!r.TryReadVariant(out var countOut)) return false;

            _vout = new TxOut[countOut];
            for (var i = 0L; i < countOut; i++)
            {
                ref var txOut = ref _vout[i];
                if (!txOut.TryParseTxOut(ref r, bp)) return false;
            }

            if (!r.TryReadLittleEndian(out _lockTime)) return false;

            var end = r.Data.Position;

            // Compute the transaction hash.
            var txBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using (var sha256 = SHA256.Create())
            {
                var hash1 = sha256.ComputeHash(txBytes);
                var hash2 = sha256.ComputeHash(hash1);
                hash2.CopyTo(_txHash.Span);
            }

            bp.TxParsed(this, r.Data.Consumed);

            return true;
        }

        public bool TryReadTransaction(ref ByteSequenceReader r)
        {
            var start = r.Data.Position;

            if (!r.TryReadLittleEndian(out _version)) goto fail;
            if (!r.TryReadVariant(out var countIn)) goto fail;

            _vin = new TxIn[countIn];
            for (var i = 0L; i < countIn; i++)
            {
                ref var txIn = ref _vin[i];
                if (!txIn.TryReadTxIn(ref r)) goto fail;
            }

            if (!r.TryReadVariant(out long countOut)) goto fail;

            _vout = new TxOut[countOut];
            for (var i = 0L; i < countOut; i++)
            {
                ref var txOut = ref _vout[i];
                if (!txOut.TryReadTxOut(ref r)) goto fail;
            }

            if (!r.TryReadLittleEndian(out _lockTime)) goto fail;

            var end = r.Data.Position;

            // Compute the transaction hash.
            var txBytes = r.Data.Sequence.Slice(start, end).ToArray();
            using (var sha256 = SHA256.Create())
            {
                var hash1 = sha256.ComputeHash(txBytes);
                var hash2 = sha256.ComputeHash(hash1);
                hash2.CopyTo(_txHash.Span);
            }

            return true;

            fail:
            return false;
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
                .Add(_version)
                .Add(_vin.Length.AsVarIntBytes())
                ;
            foreach (var txIn in _vin)
                writer
                    .Add(txIn)
                    ;
            writer
                .Add(_vout.Length.AsVarIntBytes())
                ;
            foreach (var txOut in _vout)
                writer
                    .Add(txOut)
                    ;
            writer
                .Add(_lockTime)
                ;
            return writer;
        }

        //public byte[] ToBytes()
        //{
        //    var wl = new LengthWriter();
        //    wl.Add(this);
        //    var length = wl.Length;
        //    var bytes = new byte[length];
        //    var wm = new MemoryWriter(new Memory<byte>(bytes));
        //    wm.Add(this);
        //    return bytes;
        //}
    }
}
