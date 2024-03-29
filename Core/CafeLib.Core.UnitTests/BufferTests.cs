﻿using System;
using System.Text;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;
using Xunit;

namespace CafeLib.Core.UnitTests;

public class BufferTests
{
    [Fact]
    public void ByteSpan_Concat_Test()
    {
        var encoder = new AsciiEncoder();
        var span1 = new ByteSpan(encoder.Decode("cat"));
        var span2 = new ByteSpan(encoder.Decode("doggy"));
        var concat = span1 + span2;
        var result = encoder.Encode(concat);
        Assert.Equal("catdoggy", result);

        var span3 = new ReadOnlyByteSpan(encoder.Decode("mouse"));
        concat = span1 + span3;
        result = encoder.Encode(concat);
        Assert.Equal("catmouse", result);
    }

    [Fact]
    public void ReadOnlyByteSpan_Concat_Test()
    {
        var encoder = new AsciiEncoder();
        var span1 = new ReadOnlyByteSpan(encoder.Decode("cat"));
        var span2 = new ReadOnlyByteSpan(encoder.Decode("doggy"));
        var concat = span1 + span2;
        var result = encoder.Encode(concat);
        Assert.Equal("catdoggy", result);

        var span3 = new ByteSpan(encoder.Decode("mouse"));
        concat = span1 + span3;
        result = encoder.Encode(concat);
        Assert.Equal("catmouse", result);
    }

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
    public void CharSpan_Concat_Test()
    {
        var span1 = new CharSpan("cat");
        var span2 = new CharSpan("doggy");
        var result = span1 + span2;
        Assert.Equal("catdoggy", result.ToString());

        var span3 = new ReadOnlyCharSpan("doggy");
        result = span1 + span3;
        Assert.Equal("catdoggy", result.ToString());
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
    public void ReadOnlyCharSpan_Concat_Test()
    {
        var span1 = new ReadOnlyCharSpan("cat");
        var span2 = new ReadOnlyCharSpan("doggy");
        var result = span1 + span2;
        Assert.Equal("catdoggy", result.ToString());

        var span3 = new CharSpan("doggy");
        result = span1 + span3;
        Assert.Equal("catdoggy", result);
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