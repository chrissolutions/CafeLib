using System;
using System.Buffers;

namespace CafeLib.Bitcoin.Shared.Buffers
{
    public readonly struct ReadOnlyByteSequence
    {
        public static readonly ReadOnlyByteSequence Empty = new ReadOnlyByteSequence(Array.Empty<byte>());

        public ReadOnlySequence<byte> Data { get; }

        public ReadOnlyByteSequence(byte[] data)
        {
            Data = new ReadOnlySequence<byte>(data);
        }

        public ReadOnlyByteSequence(ReadOnlySequence<byte> data)
        {
            Data = data;
        }

        public ReadOnlySequence<byte>.Enumerator GetEnumerator() => Data.GetEnumerator();

        public bool IsEmpty => Data.IsEmpty;

        public long Length => Data.Length;

        public void CopyTo(ByteSpan destination) => Data.CopyTo(destination);
        public byte[] ToArray() => Data.ToArray();

        public static implicit operator ReadOnlySequence<byte>(ReadOnlyByteSequence rhs)
        {
            return rhs.Data;
        }

        public static implicit operator ReadOnlyByteSequence(ReadOnlySequence<byte> rhs)
        {
            return new ReadOnlyByteSequence(rhs);
        }

        /// <summary>
        /// Returns true if sequence starts with other sequence, or if other sequence length is zero.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool StartsWith(ReadOnlyByteSequence other)
        {
            var s = Data;
            var o = other;
            var oLen = o.Length;

            if (oLen > s.Length) return false;

            while (oLen > 0)
            {
                var sMem = Data.First;
                var oMem = other.Data.First;
                var len = Math.Min(sMem.Length, oMem.Length);
                if (!sMem.Span.Slice(0, len).SequenceEqual(oMem.Span.Slice(0, len))) return false;
                s = s.Slice(len);
                o = o.Data.Slice(len);
                oLen = o.Length;
            }
            return true;
        }
        public ReadOnlyByteSequence RemoveSlice(long start, long end) => RemoveSlice(Data.GetPosition(start), Data.GetPosition(end));

        /// <summary>
        /// Returns a new ReadOnlySequence with a slice removed.
        /// </summary>
        /// <param name="sequence">Sequence from which to remove a slice.</param>
        /// <param name="start">Start of slice to remove.</param>
        /// <param name="end">End of slice to remove</param>
        /// <returns></returns>
        public ReadOnlySequence<byte> RemoveSlice(SequencePosition start, SequencePosition end)
        {
            var before = Data.Slice(Data.Start, start);
            var after = Data.Slice(end, Data.End);

            if (before.Length == 0)
                return after;

            if (after.Length == 0)
                return before;

            // Join before and after sequences.

            //var typeBefore = before.GetSequenceType();
            //var typeAfter = after.GetSequenceType();

            //if (typeBefore != typeAfter)
            //    throw new InvalidOperationException();

            //var first = (KzSequenceSegment<byte>)null;
            //var last = (KzSequenceSegment<byte>)null;
            //switch (typeBefore)
            //{
            //    case KzSequenceType.Segment:
            //        {
            //            foreach (var m in before)
            //            {
            //                if (last == null)
            //                    last = first = new KzSequenceSegment<byte>(m);
            //                else
            //                    last = last.Append(m);
            //            }
            //            foreach (var m in after)
            //            {
            //                last = last.Append(m);
            //            }
            //        }
            //        break;
            //    case KzSequenceType.MemoryManager:
            //    case KzSequenceType.Array:
            //    case KzSequenceType.String:
            //        {
            //            first = new KzSequenceSegment<byte>(before.First);
            //            last = first.Append(after.First);
            //        }
            //        break;
            //    default: throw new NotImplementedException();
            //}
            //return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
            return new ReadOnlySequence<byte>();
        }


    }
}
