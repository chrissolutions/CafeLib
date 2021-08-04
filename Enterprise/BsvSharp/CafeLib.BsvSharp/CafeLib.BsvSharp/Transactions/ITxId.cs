using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Transactions
{
    public interface ITxId
    {
        UInt256 TxHash { get; }
    }
}
