using System;
using System.Text;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;
using Xunit;

namespace CafeLib.Core.UnitTests;

public class BufferTests
{
    [Fact]
    public void ReadOnlyByteSpan_Slice_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var bytes = new AsciiEncoder().Decode(text);
        var span = new ReadOnlySpan<byte>(bytes);
        var roByteSpan = new ReadOnlyByteSpan(span);

        var etojSlice = roByteSpan.Slice(4, 6);
        var bytesSlice = bytes[4..10];
        Assert.Equal(0, etojSlice.SequenceCompareTo(bytesSlice));
    }

    [Fact]
    public void ReadOnlyByteSpan_Slice_ToEnd_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var bytes = new AsciiEncoder().Decode(text);
        var span = new ReadOnlySpan<byte>(bytes);
        var roByteSpan = new ReadOnlyByteSpan(span);

        var etojSlice = roByteSpan[4..];
        var bytesSlice = bytes[4..];
        Assert.Equal(0, etojSlice.SequenceCompareTo(bytesSlice));
    }

    [Fact]
    public void ReadOnlyByteSpan_Enumeration_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var bytes = new AsciiEncoder().Decode(text);
        var span = new ReadOnlySpan<byte>(bytes);
        var roByteSpan = new ReadOnlyByteSpan(span);
        var newBytes = new byte[text.Length];

        var i = 0;
        foreach (var b in roByteSpan)
        {
            newBytes[i++] = b;
        }

        Assert.Equal(0, roByteSpan.SequenceCompareTo(newBytes));
    }

    [Fact]
    public void ReadOnlyByteSpan_IsEmpty_Test()
    {
        var roByteSpan = new ReadOnlyByteSpan(null);
        Assert.True(roByteSpan.IsEmpty);
    }

    [Fact]
    public void CharSpan_Slice_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var charSpan = new CharSpan(text);

        var etojSlice = charSpan.Slice(4, 6);
        var charsSlice = charSpan[4..10];
        Assert.Equal(charsSlice.ToString(), etojSlice.ToString());
    }

    [Fact]
    public void ReadOnlyCharSpan_Slice_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var charSpan = new ReadOnlyCharSpan(text);

        var etojSlice = charSpan.Slice(4, 6);
        var charsSlice = charSpan[4..10];
        Assert.Equal(charsSlice, etojSlice);
    }

    [Fact]
    public void ReadOnlyCharSpan_Slice_ToEnd_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var charSpan = new ReadOnlyCharSpan(text);

        var etojSlice = charSpan[4..];
        var charsSlice = charSpan[4..];

        Assert.Equal(0, etojSlice.SequenceCompareTo(charsSlice));
        Assert.Equal(charsSlice, etojSlice);
    }

    [Fact]
    public void ReadOnlyCharSpan_Enumeration_Test()
    {
        const string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var charSpan = new ReadOnlyCharSpan(text);
        var sb = new StringBuilder();

        foreach(var c in charSpan)
        {
            sb.Append(c);
        }

        Assert.Equal(charSpan, sb.ToString());
    }

    [Fact]
    public void ReadOnlyCharSpan_IsEmpty_Test()
    {
        var roCharSpan = new ReadOnlyCharSpan(null);
        Assert.True(roCharSpan.IsEmpty);
    }
}