using CafeLib.BsvSharp.Numerics;

namespace CafeLib.BsvSharp.Chain
{
    public interface IChainId
    {
        UInt256 Hash { get; }
    }
}
