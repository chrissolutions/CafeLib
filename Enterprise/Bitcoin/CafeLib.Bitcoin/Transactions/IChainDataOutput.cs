using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CafeLib.Bitcoin.Transactions
{
    public interface IChainDataOutput<T>
    {
        byte[] ToBuffer();

        string ToHex();

        string ToJson();
    }
}
