#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.BsvSharp.Buffers;

namespace CafeLib.BsvSharp.Encoding
{
    /// <summary>
    /// Base class for encoders converting between strings and byte sequences.
    /// Only two methods are abstract, one Encode and one TryDecode methods.
    /// Additional overloads of each are virtual to allow optimizations to be provided.
    /// </summary>
    public abstract class EncoderBase
    {
        public string Encode(ReadOnlyByteSpan data1, ReadOnlyByteSpan data2)
        {
            var bytes = new byte[data1.Length + data2.Length].AsSpan();
            data1.CopyTo(bytes);
            data2.CopyTo(bytes.Slice(data1.Length));
            return Encode(bytes);
        }

        /// <summary>
        /// Encodes a span of bytes as a string.
        /// </summary>
        /// <param name="data">Byte sequence to be encoded.</param>
        /// <returns>String representation of byte sequence capable of being decoded back into a byte sequence.</returns>
        public abstract string Encode(ReadOnlyByteSpan data);

        /// <summary>
        /// Encodes a sequence of bytes as a string./// 
        /// </summary>
        /// <param name="data">Byte sequence to be encoded.</param>
        /// <returns></returns>
        public virtual string Encode(ReadOnlyByteSequence data) => Encode((ReadOnlyByteSpan) data);

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
        /// <param name="data">Will be reduced if longer than encoded data. If it is too small, false is returned.</param>
        /// <returns></returns>
        public virtual bool TryDecode(string encoded, ref ByteSpan data)
        {
            var ok = TryDecode(encoded, out var ba);
            if (ok && ba.Length < data.Length)
                data = data.Slice(0, ba.Length);
            if (ok && ba.Length <= data.Length)
                ba.CopyTo(data);
            else
                ok = false;
            return ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="data">Will be filled with zero if longer than encoded data. If it is too small, false is returned.</param>
        /// <returns></returns>
        public virtual bool TryDecode(string encoded, ByteSpan data)
        {
            var span = data;
            var ok = TryDecode(encoded, ref span);
            if (ok && span.Length < data.Length)
                data.Slice(span.Length).Data.Fill(0);
            return ok;
        }

        public virtual byte[] Decode(string encoded)
        {
            if (!TryDecode(encoded, out var span)) throw new ArgumentException(nameof(encoded));
            return span.ToArray();
        }
    }
}
