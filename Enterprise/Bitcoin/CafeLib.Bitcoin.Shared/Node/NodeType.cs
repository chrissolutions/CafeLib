using System.ComponentModel;

namespace CafeLib.Bitcoin.Shared.Node
{
    public enum NodeType
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