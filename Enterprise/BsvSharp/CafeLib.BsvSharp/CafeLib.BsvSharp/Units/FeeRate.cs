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
            var rate = byteCount > 0 ? (decimal)1000 * feePaid.Satoshis / byteCount : 0;
            SatoshiPerKiloBytes = new Amount(rate, BitcoinUnit.Satoshi);
        }
    }
}
