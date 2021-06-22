#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Core.Support;

namespace CafeLib.Bitcoin.Encoding
{
    /// <summary>
    /// The string begins with the first byte.
    /// Encodes a sequence of bytes as hexadecimal digits where:
    /// Character 0 corresponds to the high nibble of the first byte. 
    /// Character 1 corresponds to the low nibble of the first byte. 
    /// </summary>
    public class Utf8Encoder : SingletonBase<Utf8Encoder>, IEncoder
    {
        public byte[] Decode(string source) => System.Text.Encoding.UTF8.GetBytes(source);

        public string Encode(byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes);
    }
}
