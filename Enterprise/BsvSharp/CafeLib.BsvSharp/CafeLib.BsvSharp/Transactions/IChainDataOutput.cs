namespace CafeLib.BsvSharp.Transactions
{
    public interface IChainDataOutput<T>
    {
        byte[] ToBuffer();

        string ToHex();

        string ToJson();
    }
}
