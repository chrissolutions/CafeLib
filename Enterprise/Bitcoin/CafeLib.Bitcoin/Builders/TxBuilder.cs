using System.Collections.Generic;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Scripting;
using CafeLib.Bitcoin.Units;

namespace CafeLib.Bitcoin.Builders
{
    /// <summary>
    /// 
    /// </summary>
    public class TxBuilder
    {
        private Transaction _tx;
        private List<TxIn> _vin;
        private List<TxOut> _vout;
        private TxOutMap _txOutMap;
        private SigOperations _sigOps;
        private Script _changeScript;
        private Amount _changeAmount;
        private Amount _feePerKbAmount;
        private uint _lockTime;
        private uint _version;
        private uint sigsPerInput;
        private Amount _dust;
        private bool _isDustChangeToFees;
        private HashCache _hashCache;
    }
}
