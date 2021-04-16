using System;

namespace CafeLib.Bitcoin.Shared.Scripting
{
    [Flags]
    public enum SigHashType : byte 
    {
        UNSUPPORTED = 0,
        ALL = 1,
        NONE = 2,
        SINGLE = 3,
        FORKID = 0x40,
        ANYONECANPAY = 0x80,
    }
}