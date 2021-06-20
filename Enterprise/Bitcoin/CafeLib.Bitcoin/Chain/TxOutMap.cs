using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Chain
{
    public class TxOutMap
    {
        private IDictionary<UInt256, (UInt256, uint, TxOut)> _map = new Dictionary<UInt256, (UInt256, uint, TxOut)>();
    }
}
