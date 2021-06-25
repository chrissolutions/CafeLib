using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public interface IChainId
    {
        UInt256 Hash { get; }
    }
}
