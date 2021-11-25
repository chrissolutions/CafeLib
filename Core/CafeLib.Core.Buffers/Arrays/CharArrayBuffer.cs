namespace CafeLib.Core.Buffers.Arrays
{
    public class CharArrayBuffer : ArrayBuffer<char>
    {
        /// <summary>
        /// CharArrayBuffer default constructor
        /// </summary>
        public CharArrayBuffer()
        {
        }

        /// <summary>
        /// CharArrayBuffer constructor.
        /// </summary>
        /// <param name="chars"></param>
        public CharArrayBuffer(ReadOnlyCharSpan chars)
            : base(chars)
        {
        }

        public static implicit operator CharArrayBuffer(char[] rhs) => new(rhs);
        public static implicit operator char[](CharArrayBuffer rhs) => rhs.ToArray();
        public static implicit operator ReadOnlyCharSpan(CharArrayBuffer rhs) => rhs.Span;
    }
}
