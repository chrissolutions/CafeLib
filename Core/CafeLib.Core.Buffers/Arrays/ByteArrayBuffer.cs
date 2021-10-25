namespace CafeLib.Core.Buffers.Arrays
{
    public class ByteArrayBuffer : ArrayBuffer<byte>
    {
        /// <summary>
        /// ByteDataWriter default constructor
        /// </summary>
        public ByteArrayBuffer()
        {
        }

        /// <summary>
        /// ByteDataWriter constructor.
        /// </summary>
        /// <param name="bytes"></param>
        public ByteArrayBuffer(ReadOnlyByteSpan bytes)
            : base(bytes)
        {
        }

        public static implicit operator ByteArrayBuffer(byte[] rhs) => new(rhs);
        public static implicit operator byte[](ByteArrayBuffer rhs) => rhs.ToArray();
        public static implicit operator ReadOnlyByteSpan(ByteArrayBuffer rhs) => rhs.Span;
    }
}
