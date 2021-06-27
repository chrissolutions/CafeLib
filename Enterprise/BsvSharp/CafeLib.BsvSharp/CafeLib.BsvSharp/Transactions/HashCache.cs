using System.Collections.Generic;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public class HashCache
    {
        private IDictionary<UInt256, (UInt256, UInt256, UInt256)> _map = new Dictionary<UInt256, (UInt256, UInt256, UInt256)>();
    }
}
