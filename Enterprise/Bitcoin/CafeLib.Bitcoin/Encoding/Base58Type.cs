namespace CafeLib.Bitcoin.Encoding
{
    public enum Base58Type
    {
        PubkeyAddress,
        ScriptAddress,
        SecretKey,
        ExtPublicKey,
        ExtSecretKey,

        MaxBase58Types
    };
}