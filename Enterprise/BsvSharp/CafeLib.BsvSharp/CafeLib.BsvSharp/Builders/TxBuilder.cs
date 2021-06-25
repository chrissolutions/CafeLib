using System.Collections.Generic;
using CafeLib.Bitcoin.Units;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Crypto;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;

namespace CafeLib.BsvSharp.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class TxBuilder
    {
        private Transaction Tx { get; set; }
        private List<TxIn> Vin { get; set; }
        private List<TxOut> Vout { get; set; }
        private TxOutMap TxOutMap { get; set; }
        private SigOperations SigOps { get; set; }
        private Script ChangeScript { get; set; }
        private Amount ChangeAmount { get; set; }
        private Amount FeePerKbAmount { get; set; }
        private uint LockTime { get; set; }
        private uint Version { get; set; }
        private uint SigsPerInput { get; set; }
        private Amount Dust { get; set; }
        private bool SendDustChangeToFees { get; set; }
        private HashCache HashCache { get; set; }


        /// <summary>
        ///     Import a transaction partially signed by someone else. The only thing you
        ///     can do after this is sign one or more inputs. Usually used for multisig
        ///     transactions. uTxOutMap is optional. It is not necessary so long as you
        ///     pass in the txOut when you sign. You need to know the output when signing
        ///     an input, including the script in the output, which is why this is
        ///     necessary when signing an input.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="txOutMap"></param>
        /// <param name="signOperations"></param>
        /// <returns></returns>
        public TxBuilder ImportPartiallySignedTx(Transaction tx, TxOutMap txOutMap = null, SigOperations signOperations = null)
        {
            Tx = tx ?? Tx;
            TxOutMap = txOutMap ?? TxOutMap;
            SigOps = signOperations ?? SigOps;
            return this;
        }

        public TxBuilder InputFromScript(UInt256 txHashBuf, int txOutIndex, TxOut txOut, Script script, uint sequence)
        {
            Vin.Add(new TxIn(txHashBuf, txOutIndex, script, sequence));
            TxOutMap.Set(txHashBuf, txOutIndex, txOut);
            return this;
        }
    }
}
