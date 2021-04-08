namespace CafeLib.Bitcoin.Global
{
    public class KzTestNetParams : KzChainParams
    {
        public KzTestNetParams() : base()
        {
            NetworkId = "test";

            Base58Prefixes[(int)KzBase58Type.PubkeyAddress] = new byte[] { (111) };
            Base58Prefixes[(int)KzBase58Type.ScriptAddress] = new byte[] { (196) };
            Base58Prefixes[(int)KzBase58Type.SecretKey] = new byte[] { (239) };
            Base58Prefixes[(int)KzBase58Type.ExtPublicKey] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
            Base58Prefixes[(int)KzBase58Type.ExtSecretKey] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
        }
    }
}