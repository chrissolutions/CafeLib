using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public class TransactionInput : IChainId
    {
        public UInt256 Hash { get; }
    }
}
