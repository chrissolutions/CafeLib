using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Kafka.Common
{
    /// <summary>
    /// A BinaryWriter that stores values in BigEndian format.
    /// </summary>
    /// <remarks>
    /// Booleans, bytes and byte arrays will be written directly.
    /// All other values will be converted to a byte array in BigEndian byte order and written.
    /// Characters and Strings will all be encoded in UTF-8 (which is byte order independent).
    /// </remarks>
    /// <remarks>
    /// BigEndianBinaryWriter code provided by Zoltu
    /// https://github.com/Zoltu/Zoltu.EndianAwareBinaryReaderWriter
    /// The code was modified to implement Kafka specific byte handling.
    /// </remarks>
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream stream)
            : base(stream, Encoding.UTF8)
        {
            Contract.Requires(stream != null);
        }

        public BigEndianBinaryWriter(Stream stream, bool leaveOpen)
            : base(stream, Encoding.UTF8, leaveOpen)
        {
            Contract.Requires(stream != null);
        }

        public override void Write(decimal value)
        {
            var ints = decimal.GetBits(value);
            Contract.Assume(ints != null);
            Contract.Assume(ints.Length == 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(ints);

            for (var i = 0; i < 4; ++i)
            {
                var bytes = BitConverter.GetBytes(ints[i]);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);

                Write(bytes);
            }
        }

        public override void Write(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBigEndian(bytes);
        }

        public override void Write(string value)
        {
            throw new NotSupportedException("Kafka requires specific string length prefix encoding.");
        }

        public void Write(byte[] value, StringPrefixEncoding encoding)
        {
            if (value == null)
            {
                Write(-1);
                return;
            }

            switch (encoding)
            {
                case StringPrefixEncoding.Int16:
                    Write((Int16)value.Length);
                    break;
                case StringPrefixEncoding.Int32:
                    Write(value.Length);
                    break;
            }

            Write(value);
        }

        public void Write(string value, StringPrefixEncoding encoding)
        {
            if (value == null)
            {
                switch (encoding)
                {
                    case StringPrefixEncoding.Int16:
                        Write((short)(-1));
                        return;
                    default:
                        Write(-1);
                        return;
                }
            }

            switch (encoding)
            {
                case StringPrefixEncoding.Int16:
                    Write((short)value.Length);
                    break;
                case StringPrefixEncoding.Int32:
                    Write(value.Length);
                    break;
            }

            Write(Encoding.UTF8.GetBytes(value));
        }


        private void WriteBigEndian(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Write(bytes);
        }
    }

    public enum StringPrefixEncoding
    {
        Int16,
        Int32,
        None
    };
}
