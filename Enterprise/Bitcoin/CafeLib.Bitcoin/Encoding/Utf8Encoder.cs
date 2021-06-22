#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;

namespace CafeLib.Bitcoin.Encoding
{
    /// <summary>
    /// The string begins with the first byte.
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// Character 0 corresponds to the high nibble of the first byte. 
    /// Character 1 corresponds to the low nibble of the first byte. 
    /// </summary>
    public class Utf8Encoder : IEncoder
    {
        public string Encode(byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes);

        public byte[] Decode(string source) => TryDecode(source, out var bytes) ? bytes : throw new FormatException(nameof(source));

        public bool TryDecode(string data, out byte[] bytes)
        {
            try
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(data);
                return true;
            }
            catch
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }
    }
}
