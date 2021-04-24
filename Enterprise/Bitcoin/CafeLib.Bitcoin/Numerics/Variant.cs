#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Extensions;

namespace CafeLib.Bitcoin.Numerics
{
    public readonly struct Variant
    {
        private const int SizeofVarByte = sizeof(byte);
        private const int SizeofVarChar = sizeof(char) + sizeof(byte) ;
        private const int SizeofVarInt = sizeof(int) + sizeof(byte);
        private const int SizeofVarLong = sizeof(long) + sizeof(byte);

        public long Value { get; }
        public int Length { get; }
        public byte Prefix { get; }

        internal Variant(ulong value)
            : this((long)value)
        {
        }

        internal Variant(int value)
        {
            var (length, prefix) = GetInfo(value);
            Length = length;
            Prefix = prefix;
            Value = value;
        }

        internal Variant(long value)
        {
            var (length, prefix) = GetInfo(value);
            Length = length;
            Prefix = prefix;
            Value = value;
        }

        public static implicit operator byte[](Variant rhs) => rhs.ToArray();
        public static explicit operator Variant(int rhs) => new Variant(rhs);
        public static explicit operator Variant(uint rhs) => new Variant(rhs);
        public static explicit operator Variant(long rhs) => new Variant(rhs);
        public static explicit operator Variant(ulong rhs) => new Variant(rhs);

        public byte[] ToArray() => AsBytes(Value);

        private static byte[] AsBytes(long value)
        {
            var (len, prefix) = GetInfo(value);
            var bytes = new byte[len];
            var s = value.AsReadOnlySpan();
            switch (len)
            {
                case SizeofVarByte:
                    bytes[0] = s[0];
                    break;

                case SizeofVarChar:
                    bytes[0] = prefix;
                    bytes[1] = s[0];
                    bytes[2] = s[1];
                    break;

                case SizeofVarInt:
                    bytes[0] = prefix;
                    bytes[1] = s[0];
                    bytes[2] = s[1];
                    bytes[3] = s[2];
                    bytes[4] = s[3];
                    break;

                case SizeofVarLong:
                    bytes[0] = prefix;
                    bytes[1] = s[0];
                    bytes[2] = s[1];
                    bytes[3] = s[2];
                    bytes[4] = s[3];
                    bytes[5] = s[4];
                    bytes[6] = s[5];
                    bytes[7] = s[6];
                    bytes[8] = s[7];
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return bytes;
        }

        public static (int length, byte prefix) GetInfo(long value)
        {
            int len;
            byte prefix;
            var unsigned = (ulong)value;

            if (unsigned <= 0xfc)
            {
                len = SizeofVarByte;
                prefix = 0;
            }
            else if (unsigned <= 0xffff)
            {
                len = SizeofVarChar; 
                prefix = 0xfd; 
            }
            else if (unsigned <= 0xffff_ffff)
            {
                len = SizeofVarInt; 
                prefix = 0xfe; 
            }
            else
            {
                len = SizeofVarLong; 
                prefix = 0xff;
            }

            return (len, prefix);
        }

        /// <summary>
        /// Reads an <see cref="long"/> as in bitcoin varint format.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        /// <returns>False if there wasn't enough data for an <see cref="ulong"/>.</returns>
        public static bool TryRead(ref ByteSequenceReader reader, out ulong value)
        {
            value = 0;
            if (!TryRead(ref reader, out long tempValue)) return false;
            value = (ulong) tempValue;
            return true;
        }

        /// <summary>
        /// Reads an <see cref="long"/> as in bitcoin varint format.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="long"/>.</returns>
        public static bool TryRead(ref ByteSequenceReader reader, out long value) 
        {
			value = 0L;

            var b = reader.TryRead(out var b0);
			if (!b) return false;

			if (b0 <= 0xfc) 
            {
				value = b0;
			}
            else if (b0 == 0xfd) 
            {
				b = reader.TryReadLittleEndian(out short v16);
				value = v16;
			}
            else if (b0 == 0xfe) 
            {
				b = reader.TryReadLittleEndian(out int v32);
				value = v32;
			}
            else 
            {
				b = reader.TryReadLittleEndian(out value);
			}

			return b;
		}
    }
}
