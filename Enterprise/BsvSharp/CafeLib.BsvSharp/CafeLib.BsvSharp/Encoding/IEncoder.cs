namespace CafeLib.BsvSharp.Encoding
{
    public interface IEncoder
    {
        /// <summary>
        /// Encodes source bytes to a string./// 
        /// </summary>
        /// <param name="source">Byte source to be encoded.</param>
        /// <returns></returns>
        string Encode(byte[] source);

        /// <summary>
        /// Decode hex string to byte array.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        byte[] Decode(string source);
    }
}
