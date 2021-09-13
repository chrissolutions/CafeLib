namespace CafeLib.Core.Encodings
{
    public interface IEncoder
    {
        /// <summary>
        /// Encodes source bytes to a string.
        /// </summary>
        /// <param name="source">Byte source to be encoded.</param>
        /// <returns>encoded string</returns>
        string Encode(byte[] source);

        /// <summary>
        /// Decode string to byte array.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>decoded byte array</returns>
        byte[] Decode(string source);
    }
}