using System;

namespace CafeLib.Bitcoin.Shared.Scripting
{
    [Flags]
    public enum SignatureHashEnum : byte
    {
        Unsupported = 0,
        All = 1,
        None = 2,
        Single = 3,
        Forkid = 0x40,
        AnyoneCanPay = 0x80,
    }
}