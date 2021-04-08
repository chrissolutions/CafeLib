namespace CafeLib.Bitcoin.Global
{
    public class KzTestNetParams : KzChainParams
    {
        public KzTestNetParams() : base()
        {
            NetworkId = "test";

            Base58Prefixes[(int)KzBase58Type.PUBKEY_ADDRESS] = new byte[] { (111) };
            Base58Prefixes[(int)KzBase58Type.SCRIPT_ADDRESS] = new byte[] { (196) };
            Base58Prefixes[(int)KzBase58Type.SECRET_KEY] = new byte[] { (239) };
            Base58Prefixes[(int)KzBase58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            Base58Prefixes[(int)KzBase58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
        }
    }
}