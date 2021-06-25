namespace CafeLib.Bitcoin.Transactions
{
    public interface IChainDataInput<out T>
    {
        T ParseBuffer(byte[] buffer);
        T ParseHex(string hex);
        T ParseJson(string json);
    }
}