using System.ComponentModel;

namespace CafeLib.Bitcoin.Shared
{
    public enum ChainType
    {
        Unknown,
        [Description("main")]
        Main,

        [Description("test")]
        Test,

        [Description("regtest")]
        Regtest,

        [Description("stn")]
        Stn
    }
}