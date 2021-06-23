using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Scripting;
using CafeLib.Bitcoin.Units;

namespace CafeLib.Bitcoin.Chain
{
    public class Utxo
    {
        public UInt256 TxHash { get; set; }
        private OutPoint OutPoint { get; set; }
        public Amount Amount { get; set; }
        public Script Script { get; set; }
        private int Height { get; set; }
        private bool IsCoinbase { get; set; }
        private string Address { get; set; }

        /// <summary>
        ///  Creates a stored transaction output.
        /// </summary>
        /// <param name="txHash">The hash of the containing transaction</param>
        /// <param name="outPoint">The outpoint</param>
        /// <param name="amount">The amount available</param>
        /// <param name="height">The height this output was created in</param>
        /// <param name="isCoinbase">The coinbase flag</param>
        /// <param name="script">script</param>
        public Utxo(UInt256 txHash, OutPoint outPoint, Amount amount, int height, bool isCoinbase, Script script)
        {
            TxHash = txHash;
            OutPoint = outPoint;
            Amount = amount;
            Height = height;
            IsCoinbase = isCoinbase;
            Script = script;
            Address = "";
        }

        /// <summary>
        /// Creates a stored transaction output.
        /// </summary>
        /// <param name="txHash">The hash of the containing transaction</param>
        /// <param name="outPoint">The outpoint</param>
        /// <param name="amount">The amount available</param>
        /// <param name="height">The height this output was created in</param>
        /// <param name="isCoinbase">The coinbase flag</param>
        /// <param name="script">script</param>
        /// <param name="address">The address</param>
        public Utxo(UInt256 txHash, OutPoint outPoint, Amount amount, int height, bool isCoinbase, Script script, string address)
            : this(txHash, outPoint, amount, height, isCoinbase, script)
        {
            Address = address;
        }
    }
}
