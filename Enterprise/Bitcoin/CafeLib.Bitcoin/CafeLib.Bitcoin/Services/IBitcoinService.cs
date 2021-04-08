using CafeLib.Bitcoin.Global;

namespace CafeLib.Bitcoin.Services
{
    public interface IBitcoinService
    {
        KzChainParams Params { get; }
    }
}
