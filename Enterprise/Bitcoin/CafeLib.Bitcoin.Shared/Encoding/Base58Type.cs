namespace CafeLib.Bitcoin.Shared.Encoding
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