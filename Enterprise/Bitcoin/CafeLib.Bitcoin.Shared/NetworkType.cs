using System.ComponentModel;

namespace CafeLib.Bitcoin.Shared
{
    public enum NetworkType
    {
        Unknown,
        [Description("main")]
        Main,

        [Description("test")]
        Test,

        [Description("regtest")]
        Regression,

        [Description("stn")]
        Scaling
    }
}