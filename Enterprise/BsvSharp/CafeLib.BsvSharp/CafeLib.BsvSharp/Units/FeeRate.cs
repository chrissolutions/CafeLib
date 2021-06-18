using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.BsvSharp.Units
{
    public class FeeRate
    {
        public Amount SatoshiPerKiloBytes { get; }

        public FeeRate(Amount feePaid, ulong byteCount)
        {
            SatoshiPerKiloBytes = byteCount > 0 ? (decimal)1000 * feePaid.Satoshis / byteCount : 0;
        }
    }
}
