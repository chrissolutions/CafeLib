#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;

namespace CafeLib.Bitcoin.Encoding
{
    /// <summary>
    /// The string begins with the first byte.
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// Character 0 corresponds to the high nibble of the first byte. 
    /// Character 1 corresponds to the low nibble of the first byte. 
    /// </summary>
    public class Utf8Encoder : Encoder
    {
        public override string Encode(ReadOnlyByteSequence data) => System.Text.Encoding.UTF8.GetString(data);

        public override string Encode(ReadOnlyByteSpan data) => System.Text.Encoding.UTF8.GetString(data);

        public override bool TryDecode(string data, ByteSpan span)
        {
            try
            {
                Decode(data).CopyTo(span);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool TryDecode(string data, out byte[] bytes)
        {
            bytes = default;
            try
            {
                bytes = Decode(data);
                return true;
            }
            catch
            {
                bytes = default;
                return false;
            }
        }

        public override byte[] Decode(string encoded)
        {
            return System.Text.Encoding.UTF8.GetBytes(encoded);
        }
    }
}
