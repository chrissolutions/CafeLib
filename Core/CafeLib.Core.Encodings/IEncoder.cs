namespace CafeLib.Core.Encodings
{
    public interface IEncoder
    {
        /// <summary>
        /// Encodes source bytes to a string.
        /// </summary>
        /// <param name="source">byte array to be encoded.</param>
        /// <returns>encoded string</returns>
        string Encode(byte[] source);

        /// <summary>
        /// Decode string to byte array.
        /// </summary>
        /// <param name="source">encoded string</param>
        /// <returns>decoded byte array</returns>
        byte[] Decode(string source);


        /// <summary>
        /// Decode string to byte array without exception.
        /// </summary>
        /// <param name="encoded">encoded string</param>
        /// <param name="bytes">byte array output value</param>
        /// <returns>true if successful; false otherwise</returns>
        public bool TryDecode(string encoded, out byte[] bytes);
    }
}