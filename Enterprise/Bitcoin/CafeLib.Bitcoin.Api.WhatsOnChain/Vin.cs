namespace CafeLib.Bitcoin.Api.WhatsOnChain
{
    public class Vin
    {
        public string txid;
        public int vout;
        public ScriptSig scriptSig;
        public uint sequence;
        public string coinbase;
    }
}