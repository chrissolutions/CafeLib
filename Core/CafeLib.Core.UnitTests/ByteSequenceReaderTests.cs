using System.Collections.Generic;
using System.Text;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;
using CafeLib.Core.Numerics;
using Xunit;

namespace CafeLib.Core.UnitTests;

public class ByteSequenceReaderTests
{
    protected readonly string[] HexStringFragments =
    {
        "01000000", // int
        "82bb869cf3a793432a66e826e05a6fc37469f8efb7421dc88067010000000000", // UInt256
        "7f16c5962e8bd963659c793ce370d95f093bc7e367117b3c30c1f8fdd0d97287",  // UInt256
        "76381b4d", // uint
        "4c86041b", // uint
        "554b8529", // uint
        "07000000", // uint
        "04", // int
        "3612262624047ee87660be1a707519a443b1c1ce3d248cbfc6c15870f6c5daa2", // UInt256
        "019f5b01d4195ecbc9398fbf3c3b1fa9bb3183301d7a1fb3bd174fcfa40a2b65", // UInt256
        "41ed70551dd7e841883ab8f0b16bf04176b7d1480e4f0af9f3d4c3595768d068", // UInt256
        "20d2a7bc994987302e5b1ac80fc425fe25f8b63169ea78e68fbaaefa59379bbf", // UInt256
        "03", // int
        "1d2d3d" // byte
    };

    [Fact]
    public void ByteSequenceTest()
    {
        var bytes = ReadBytes(HexStringFragments);
        var sequence = new ReadOnlyByteSequence(bytes);
        Assert.Equal(bytes.Length, sequence.Length);
    }

    [Fact]
    public void ByteSequenceReader_Test()
    {
        var bytes = ReadBytes(HexStringFragments);
        var sequence = new ReadOnlyByteSequence(bytes);
        Assert.Equal(bytes.Length, sequence.Length);

        var reader = new ByteSequenceReader(sequence);
        Assert.Equal(bytes.Length, reader.Length);

        var result = reader.TryReadLittleEndian(out int version);
        Assert.True(result);
        Assert.Equal(1, version);

        var destination = UInt256.Zero;
        result = TryReadUInt256(ref reader, ref destination, true);
        Assert.True(result);
        Assert.Equal(UInt256.FromHex("82bb869cf3a793432a66e826e05a6fc37469f8efb7421dc88067010000000000"), destination);
    }

    #region Helpers

    private static byte[] ReadBytes(IEnumerable<string> fragments)
    {
        var sb = new StringBuilder();
        foreach (var str in fragments)
        {
            sb.Append(str);
        }
        var encoder = new HexEncoder();
        return encoder.Decode(sb.ToString());
    }

    /// <summary>
    /// Reads an <see cref="UInt256"/> as in bitcoin VarInt format.
    /// </summary>
    /// <param name="reader">byte sequence reader</param>
    /// <param name="destination"></param>
    /// <param name="reverse"></param>
    /// <returns></returns>
    private static bool TryReadUInt256(ref ByteSequenceReader reader, ref UInt256 destination, bool reverse = false)
    {
        var span = destination.Span;
        if (!reader.TryCopyTo(span)) return false;
        if (reverse) span.Reverse();
        reader.Advance(span.Length);
        return true;
    }

    #endregion
}