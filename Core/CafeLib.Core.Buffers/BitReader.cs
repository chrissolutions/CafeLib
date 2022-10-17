using System.Collections;
using System.Text;

namespace CafeLib.Core.Buffers
{
    public struct BitReader
    {
        private static readonly BitArray Empty = new(0);

        private readonly BitArray _array = Empty;
        
        public int Count => _array.Length;

        public int Position { get; set; } = 0;

        public BitReader(byte[] data, int bitCount)
            : this()
        {
            var writer = new BitWriter();
            writer.Write(data, bitCount);
            _array = writer.ToBitArray();
        }

        public BitReader(BitArray array)
        {
            _array = new BitArray(array.Length);
            for (int i = 0; i < array.Length; i++)
                _array.Set(i, array.Get(i));
        }

        public bool Read()
        {
            var v = _array.Get(Position);
            Position++;
            return v;
        }

        public uint ReadUInt(int bitCount)
        {
            uint value = 0;
            for (int i = 0; i < bitCount; i++)
            {
                var v = Read() ? 1U : 0U;
                value += v << i;
            }
            return value;
        }

        public BitArray ToBitArray()
        {
            var result = new BitArray(_array.Length);
            for (int i = 0; i < _array.Length; i++)
                result.Set(i, _array.Get(i));
            return result;
        }

        public BitWriter ToWriter()
        {
            var writer = new BitWriter();
            writer.Write(_array);
            return writer;
        }

        public void Consume(int count)
        {
            Position += count;
        }

        public bool Same(BitReader b)
        {
            while (Position != Count && b.Position != b.Count)
            {
                var valuea = Read();
                var valueb = b.Read();
                if (valuea != valueb)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(_array.Length);
            for (int i = 0; i < Count; i++)
            {
                if (i != 0 && i % 8 == 0)
                    builder.Append(' ');

                builder.Append(_array.Get(i) ? "1" : "0");
            }

            return builder.ToString();
        }
    }
}
