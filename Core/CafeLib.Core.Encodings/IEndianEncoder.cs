using CafeLib.Core.Buffers;

namespace CafeLib.Core.Encodings
{
    public interface IEndianEncoder : IEncoder
    {
        int LittleEndianInt32(ReadOnlyByteSpan data);
        int BigEndianInt32(ReadOnlyByteSpan data);

        long LittleEndianInt64(ReadOnlyByteSpan data);
        long BigEndianInt64(ReadOnlyByteSpan data);
    }
}