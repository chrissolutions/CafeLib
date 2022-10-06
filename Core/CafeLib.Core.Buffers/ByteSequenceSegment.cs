using System;
using System.Buffers;
using System.Diagnostics;

namespace CafeLib.Core.Buffers
{
    public class ByteSequenceSegment : ReadOnlySequenceSegment<byte>
    {
        public ByteSequenceSegment(byte[] array, int start = 0, int length = -1)
        {
            if (length == -1) length = array.Length - start;
            Memory = new Memory<byte>(array, start, length);
        }

        public ByteSequenceSegment(ReadOnlyMemory<byte> memory)
        {
            Memory = memory;
        }

        public ByteSequenceSegment Append(ByteSequenceSegment nextSegment)
        {
            Trace.Assert(nextSegment.RunningIndex == 0);
            Next = nextSegment;
            nextSegment.RunningIndex = RunningIndex + nextSegment.Memory.Length;
            return nextSegment;
        }

        public ByteSequenceSegment Append(byte[] array, int start = 0, int length = -1)
            => Append(new ByteSequenceSegment(array, start, length));

        public ByteSequenceSegment Append(ReadOnlyMemory<byte> memory)
        {
            var segment = new ByteSequenceSegment(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };
            Next = segment;
            return segment;
        }

        public ReadOnlyByteSequence ToSequence()
        {
            var last = this;
            while (Next != null) 
                last = last?.Next as ByteSequenceSegment;

            return last != null 
                ? new ReadOnlySequence<byte>(this, 0, last, last.Memory.Length - 1) 
                : ReadOnlySequence<byte>.Empty;
        }
    }
}
