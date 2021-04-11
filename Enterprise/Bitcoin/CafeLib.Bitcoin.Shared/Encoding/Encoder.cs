#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.Bitcoin.Shared.Buffers;

namespace CafeLib.Bitcoin.Shared.Encoding
{
    /// <summary>
    /// Base class for encoders converting between strings and byte sequences.
    /// Only two methods are abstract, one Encode and one TryDecode methods.
    /// Additional overloads of each are virtual to allow optimizations to be provided.
    /// </summary>
    public abstract class Encoder
    {
        public string Encode(ReadOnlyByteSpan bytes1, ReadOnlyByteSpan bytes2)
        {
            var bytes = new byte[bytes1.Length + bytes2.Length].AsSpan();
            bytes1.CopyTo(bytes);
            bytes2.CopyTo(bytes.Slice(bytes1.Length));
            return Encode(bytes);
        }

        /// <summary>
        /// Encodes a span of bytes as a string.
        /// </summary>
        /// <param name="bytes">Byte sequence to be encoded.</param>
        /// <returns>String representation of byte sequence capable of being decoded back into a byte sequence.</returns>
        public abstract string Encode(ReadOnlyByteSpan bytes);

        /// <summary>
        /// Encodes a sequence of bytes as a string./// 
        /// </summary>
        /// <param name="bytes">Byte sequence to be encoded.</param>
        /// <returns></returns>
        public virtual string Encode(ReadOnlyByteSequence bytes) => Encode(bytes: (ReadOnlyByteSpan) bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public virtual string Encode(byte[] bytes) => Encode(bytes.AsSpan());

        /// <summary>
        /// Returns false on most failures and does not assume size of byte sequence output
        /// is easily computed from size of string input.
        /// </summary>
        /// <param name="encoded">Encoded string representation of the desired byte sequence.</param>
        /// <param name="bytes">decoded byte sequence</param>
        /// <returns>
        ///     true on success in which case bytes is non-null and valid.
        ///     false on failure in which case bytes may be null and is not valid.
        ///     bytes is the decoded byte sequence.
        /// </returns>
        public abstract bool TryDecode(string encoded, out byte[] bytes);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="bytes">Will be reduced if longer than encoded data. If it is too small, false is returned.</param>
        /// <returns></returns>
        public virtual bool TryDecode(string encoded, ref Span<byte> bytes)
        {
            var ok = TryDecode(encoded, out var ba);
            if (ok && ba.Length < bytes.Length)
                bytes = bytes.Slice(0, ba.Length);
            if (ok && ba.Length <= bytes.Length)
                ba.CopyTo(bytes);
            else
                ok = false;
            return ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="bytes">Will be filled with zero if longer than encoded data. If it is too small, false is returned.</param>
        /// <returns></returns>
        public virtual bool TryDecode(string encoded, Span<byte> bytes)
        {
            var span = bytes;
            var ok = TryDecode(encoded, ref span);
            if (ok && span.Length < bytes.Length)
                    bytes.Slice(span.Length).Fill(0);
            return ok;
        }

        public virtual byte[] Decode(string encoded)
        {
            if (!TryDecode(encoded, out var span)) throw new ArgumentException(nameof(encoded));
            return span.ToArray();
        }
    }
}
