using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Crypto
{
    public class HashCache
    {
        private IDictionary<UInt256, (UInt256, UInt256, UInt256)> _map = new Dictionary<UInt256, (UInt256, UInt256, UInt256)>();
    }
}
