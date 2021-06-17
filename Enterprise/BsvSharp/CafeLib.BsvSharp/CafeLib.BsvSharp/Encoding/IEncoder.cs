namespace CafeLib.BsvSharp.Encoding
{
    public interface IEncoder
    {
        byte[] Decode(string source);

        string Encode(byte[] source);
    }
}