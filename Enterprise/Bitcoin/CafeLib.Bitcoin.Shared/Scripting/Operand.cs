#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Persistence;

namespace CafeLib.Bitcoin.Shared.Scripting
{
    public struct Operand
    {
        Opcode _code;
        ValType _data;

        public Opcode Code => _code;
        public ValType Data => _data;

        public string CodeName => GetOpName(_code);

        public int LengthBytesCount => _code == Opcode.OP_PUSHDATA1 ? 1 : _code == Opcode.OP_PUSHDATA2 ? 2 : _code == Opcode.OP_PUSHDATA4 ? 4 : 0;

        public long Length => 1 + _data.Length + LengthBytesCount;

        public Operand(Opcode code, ValType data) { _code = code; _data = data; }

        public Operand(Opcode code) { _code = code; _data = ValType.None; }

        public static Operand Push(ReadOnlySpan<byte> data)
        {
            var code = Opcode.OP_INVALIDOPCODE;
            var val = ValType.None;
            if (data.Length == 1 && data[0] <= 16)
            {
            }
            else
            {
                if (data.Length < (int)Opcode.OP_PUSHDATA1) {
                    code = (Opcode)data.Length;
                } else if (data.Length <= 0xff) {
                    code = Opcode.OP_PUSHDATA1;
                } else if (data.Length <= 0xffff) {
                    code = Opcode.OP_PUSHDATA2;
                } else {
                    code = Opcode.OP_PUSHDATA4;
                }
                val = new ValType(data.ToArray());
            }
            var op = new Operand(code, val);
            return op;
        }

        public static Operand Push(long v)
        {
            Opcode code;
            var val = ValType.None;

            if (v == -1) 
            {
                code = Opcode.OP_1NEGATE;
            }
            else if (v >= 0 && v <= 16) 
            {
                code = v == 0 ? Opcode.OP_0 : (Opcode)(v - 1 + (int)Opcode.OP_1);
            }
            else
            {
                var bytes = BitConverter.GetBytes(v).AsSpan();
                if (v <= 0xff) 
                {
                    code = Opcode.OP_PUSH1;
                    val = new ValType(bytes.Slice(0, 1).ToArray());
                }
                else if (v <= 0xffff) 
                {
                    code = Opcode.OP_PUSH2;
                    val = new ValType(bytes.Slice(0, 2).ToArray());
                }
                else if (v <= 0xffffff) 
                {
                    code = Opcode.OP_PUSH3;
                    val = new ValType(bytes.Slice(0, 3).ToArray());
                }
                else 
                {
                    code = Opcode.OP_PUSH4;
                    val = new ValType(bytes.Slice(0, 4).ToArray());
                }
            }
            var op = new Operand(code, val);
            return op;
        }

        public bool TryCopyTo(ref Span<byte> span)
        {
            var length = Length;
            if (length > span.Length)
                return false;
            span[0] = (byte)_code;
            span = span.Slice(1);
            length = _data.Length;
            if (_code >= Opcode.OP_PUSHDATA1 && _code <= Opcode.OP_PUSHDATA4) {
                if (!BitConverter.IsLittleEndian) return false;
                var lengthBytes = BitConverter.GetBytes((uint)_data.Length).AsSpan(0, LengthBytesCount);
                lengthBytes.CopyTo(span);
                span = span.Slice(lengthBytes.Length);
            }
            if (length > 0) {
                _data.GetReader().TryCopyTo(span.Slice(0, (int)_data.Length));
                span = span.Slice((int)length);
            }
            return true;
        }

        public IBitcoinWriter AddTo(IBitcoinWriter w)
        {
            w.Add((byte)_code);
            if (_code >= Opcode.OP_PUSHDATA1 && _code <= Opcode.OP_PUSHDATA4) 
            {
                var lengthBytes = BitConverter.GetBytes((uint)_data.Length).AsSpan(0, LengthBytesCount);
                w.Add(lengthBytes);
            }

            if (_data.Length > 0)
                w.Add(_data.Sequence);

            return w;
        }

        public byte[] GetDataBytes() => _data.Sequence.ToArray();

        public byte[] GetBytes()
        {
            var bytes = new byte[Length];
            bytes[0] = (byte)_code;
            if (bytes.Length > 1)
                _data.GetReader().TryCopyTo(bytes.AsSpan().Slice(1));
            return bytes;
        }

        /*
            // script.h lines 527-562
            bool GetOp2(const_iterator &pc, opcodetype &opcodeRet,
                std::vector<uint8_t> *pvchRet) const {
                opcodeRet = OP_INVALIDOPCODE;
                if (pvchRet) pvchRet->clear();
                if (pc >= end()) return false;

                // Read instruction
                if (end() - pc < 1) return false;
                unsigned int opcode = *pc++;

                // Immediate operand
                if (opcode <= OP_PUSHDATA4) {
                    unsigned int nSize = 0;
                    if (opcode < OP_PUSHDATA1) {
                        nSize = opcode;
                    } else if (opcode == OP_PUSHDATA1) {
                        if (end() - pc < 1) return false;
                        nSize = *pc++;
                    } else if (opcode == OP_PUSHDATA2) {
                        if (end() - pc < 2) return false;
                        nSize = ReadLE16(&pc[0]);
                        pc += 2;
                    } else if (opcode == OP_PUSHDATA4) {
                        if (end() - pc < 4) return false;
                        nSize = ReadLE32(&pc[0]);
                        pc += 4;
                    }
                    if (end() - pc < 0 || (unsigned int)(end() - pc) < nSize)
                        return false;
                    if (pvchRet) pvchRet->assign(pc, pc + nSize);
                    pc += nSize;
                }

                opcodeRet = (opcodetype)opcode;
                return true;
            }
        */

        public static (bool ok, Operand op) TryRead(ref ReadOnlySequence<byte> ros, out long consumed) {
            var op = new Operand();
            var ok = op.TryReadOp(ref ros, out consumed);
            return (ok, op);
        }

        public bool TryReadOp(ref ReadOnlySequence<byte> ros) => TryReadOp(ref ros, out _);

        public bool TryReadOp(ref ReadOnlySequence<byte> ros, out long consumed)
        {
            consumed = 0L;
            var r = new SequenceReader<byte>(ros);
            if (!TryReadOp(ref r)) goto fail;

            consumed = r.Consumed;
            ros = ros.Slice(r.Consumed);

            return true;
        fail:
            return false;
        }

        public bool TryReadOp(ref SequenceReader<byte> r)
        {
            _code = Opcode.OP_INVALIDOPCODE;
            _data = ValType.None;

            if (!r.TryRead(out byte opcode)) goto fail;

            _code = (Opcode)opcode;

            // Opcodes OP_0 and OP_1 to OP_16 are single byte opcodes that push the corresponding value.
            // Opcodes from zero to 0x4b [0..75] are single byte push commands where the value is the number of bytes to push.
            // Opcode 0x4c (76) takes the next byte as the count and should be used for pushing [76..255] bytes.
            // Opcode 0x4d (77) takes the next two bytes. Used for pushing [256..65536] bytes.
            // Opcode 0x4e (78) takes the next four bytes. Used for pushing [65537..4,294,967,296] bytes.
            
            if (opcode <= (byte)Opcode.OP_PUSHDATA4) 
            {
                var nSize = 0U;
                if (opcode < (byte)Opcode.OP_PUSHDATA1) 
                {
                    nSize = opcode;
                } 
                else if (opcode == (byte)Opcode.OP_PUSHDATA1)
                {
                    if (!r.TryRead(out byte size1)) goto fail;
                    nSize = size1;
                } 
                else if (opcode == (byte)Opcode.OP_PUSHDATA2)
                {
                    if (!r.TryReadLittleEndian(out UInt16 size2)) goto fail;
                    nSize = size2;
                } 
                else if (opcode == (byte)Opcode.OP_PUSHDATA4)
                {
                    if (!r.TryReadLittleEndian(out UInt32 size4)) goto fail;
                    nSize = size4;
                }

                if (nSize >= 0)
                {
                    if (r.Remaining < nSize) goto fail;
                    _data = new ValType(r.Sequence.Slice(r.Position, (Int32)nSize));
                    r.Advance(nSize);
                }
            }
            return true;

        fail:
            return false;
        }

        public static string GetOpName(Opcode opcode)
        {
            return opcode switch
            {
                // push value
                Opcode.OP_0 => "0",
                Opcode.OP_PUSHDATA1 => "OP_PUSHDATA1",
                Opcode.OP_PUSHDATA2 => "OP_PUSHDATA2",
                Opcode.OP_PUSHDATA4 => "OP_PUSHDATA4",
                Opcode.OP_1NEGATE => "-1",
                Opcode.OP_RESERVED => "OP_RESERVED",
                Opcode.OP_1 => "1",
                Opcode.OP_2 => "2",
                Opcode.OP_3 => "3",
                Opcode.OP_4 => "4",
                Opcode.OP_5 => "5",
                Opcode.OP_6 => "6",
                Opcode.OP_7 => "7",
                Opcode.OP_8 => "8",
                Opcode.OP_9 => "9",
                Opcode.OP_10 => "10",
                Opcode.OP_11 => "11",
                Opcode.OP_12 => "12",
                Opcode.OP_13 => "13",
                Opcode.OP_14 => "14",
                Opcode.OP_15 => "15",
                Opcode.OP_16 => "16",

                // control
                Opcode.OP_NOP => "OP_NOP",
                Opcode.OP_VER => "OP_VER",
                Opcode.OP_IF => "OP_IF",
                Opcode.OP_NOTIF => "OP_NOTIF",
                Opcode.OP_VERIF => "OP_VERIF",
                Opcode.OP_VERNOTIF => "OP_VERNOTIF",
                Opcode.OP_ELSE => "OP_ELSE",
                Opcode.OP_ENDIF => "OP_ENDIF",
                Opcode.OP_VERIFY => "OP_VERIFY",
                Opcode.OP_RETURN => "OP_RETURN",

                // stack ops
                Opcode.OP_TOALTSTACK => "OP_TOALTSTACK",
                Opcode.OP_FROMALTSTACK => "OP_FROMALTSTACK",
                Opcode.OP_2DROP => "OP_2DROP",
                Opcode.OP_2DUP => "OP_2DUP",
                Opcode.OP_3DUP => "OP_3DUP",
                Opcode.OP_2OVER => "OP_2OVER",
                Opcode.OP_2ROT => "OP_2ROT",
                Opcode.OP_2SWAP => "OP_2SWAP",
                Opcode.OP_IFDUP => "OP_IFDUP",
                Opcode.OP_DEPTH => "OP_DEPTH",
                Opcode.OP_DROP => "OP_DROP",
                Opcode.OP_DUP => "OP_DUP",
                Opcode.OP_NIP => "OP_NIP",
                Opcode.OP_OVER => "OP_OVER",
                Opcode.OP_PICK => "OP_PICK",
                Opcode.OP_ROLL => "OP_ROLL",
                Opcode.OP_ROT => "OP_ROT",
                Opcode.OP_SWAP => "OP_SWAP",
                Opcode.OP_TUCK => "OP_TUCK",

                // splice ops
                Opcode.OP_CAT => "OP_CAT",
                Opcode.OP_SPLIT => "OP_SPLIT",
                Opcode.OP_NUM2BIN => "OP_NUM2BIN",
                Opcode.OP_BIN2NUM => "OP_BIN2NUM",
                Opcode.OP_SIZE => "OP_SIZE",

                // bit logic
                Opcode.OP_INVERT => "OP_INVERT",
                Opcode.OP_AND => "OP_AND",
                Opcode.OP_OR => "OP_OR",
                Opcode.OP_XOR => "OP_XOR",
                Opcode.OP_EQUAL => "OP_EQUAL",
                Opcode.OP_EQUALVERIFY => "OP_EQUALVERIFY",
                Opcode.OP_RESERVED1 => "OP_RESERVED1",
                Opcode.OP_RESERVED2 => "OP_RESERVED2",

                // numeric
                Opcode.OP_1ADD => "OP_1ADD",
                Opcode.OP_1SUB => "OP_1SUB",
                Opcode.OP_2MUL => "OP_2MUL",
                Opcode.OP_2DIV => "OP_2DIV",
                Opcode.OP_NEGATE => "OP_NEGATE",
                Opcode.OP_ABS => "OP_ABS",
                Opcode.OP_NOT => "OP_NOT",
                Opcode.OP_0NOTEQUAL => "OP_0NOTEQUAL",
                Opcode.OP_ADD => "OP_ADD",
                Opcode.OP_SUB => "OP_SUB",
                Opcode.OP_MUL => "OP_MUL",
                Opcode.OP_DIV => "OP_DIV",
                Opcode.OP_MOD => "OP_MOD",
                Opcode.OP_LSHIFT => "OP_LSHIFT",
                Opcode.OP_RSHIFT => "OP_RSHIFT",
                Opcode.OP_BOOLAND => "OP_BOOLAND",
                Opcode.OP_BOOLOR => "OP_BOOLOR",
                Opcode.OP_NUMEQUAL => "OP_NUMEQUAL",
                Opcode.OP_NUMEQUALVERIFY => "OP_NUMEQUALVERIFY",
                Opcode.OP_NUMNOTEQUAL => "OP_NUMNOTEQUAL",
                Opcode.OP_LESSTHAN => "OP_LESSTHAN",
                Opcode.OP_GREATERTHAN => "OP_GREATERTHAN",
                Opcode.OP_LESSTHANOREQUAL => "OP_LESSTHANOREQUAL",
                Opcode.OP_GREATERTHANOREQUAL => "OP_GREATERTHANOREQUAL",
                Opcode.OP_MIN => "OP_MIN",
                Opcode.OP_MAX => "OP_MAX",
                Opcode.OP_WITHIN => "OP_WITHIN",

                // crypto
                Opcode.OP_RIPEMD160 => "OP_RIPEMD160",
                Opcode.OP_SHA1 => "OP_SHA1",
                Opcode.OP_SHA256 => "OP_SHA256",
                Opcode.OP_HASH160 => "OP_HASH160",
                Opcode.OP_HASH256 => "OP_HASH256",
                Opcode.OP_CODESEPARATOR => "OP_CODESEPARATOR",
                Opcode.OP_CHECKSIG => "OP_CHECKSIG",
                Opcode.OP_CHECKSIGVERIFY => "OP_CHECKSIGVERIFY",
                Opcode.OP_CHECKMULTISIG => "OP_CHECKMULTISIG",
                Opcode.OP_CHECKMULTISIGVERIFY => "OP_CHECKMULTISIGVERIFY",

                // expansion
                Opcode.OP_NOP1 => "OP_NOP1",
                Opcode.OP_CHECKLOCKTIMEVERIFY => "OP_CHECKLOCKTIMEVERIFY",
                Opcode.OP_CHECKSEQUENCEVERIFY => "OP_CHECKSEQUENCEVERIFY",
                Opcode.OP_NOP4 => "OP_NOP4",
                Opcode.OP_NOP5 => "OP_NOP5",
                Opcode.OP_NOP6 => "OP_NOP6",
                Opcode.OP_NOP7 => "OP_NOP7",
                Opcode.OP_NOP8 => "OP_NOP8",
                Opcode.OP_NOP9 => "OP_NOP9",
                Opcode.OP_NOP10 => "OP_NOP10",

                Opcode.OP_INVALIDOPCODE => "OP_INVALIDOPCODE",

                // Note:
                //  The template matching params OP_SMALLINTEGER/etc are defined in
                //  opcodetype enum as kind of implementation hack, they are *NOT*
                //  real opcodes. If found in real Script, just let the default:
                //  case deal with them.

                _ => "OP_UNKNOWN"
            };
        }

        public string ToVerboseString()
        {
            var s = _code.ToString();

            var len = Data.Length;
            if (len > 0)
                s += " " + Encoders.Hex.Encode(Data.Sequence);
            return s;
        }

        public override string ToString()
        {
            var len = Data.Length;
            string s;
            if (len == 0)
                s = CodeName;
            else if (len < 100)
                s = Encoders.Hex.Encode(Data.Sequence);
            else
            {
                var start = Encoders.Hex.Encode(Data.Sequence.Data.Slice(0, 32));
                var end = Encoders.Hex.Encode(Data.Sequence.Data.Slice(len - 32));
                s = $"{start}...[{Data.Length} bytes]...{end}";
            }
            return s;
        }

        public override int GetHashCode() => Code.GetHashCode() ^ Data.GetHashCode();
        public override bool Equals(object obj) => obj is Operand op && this == op;
        public bool Equals(Operand op) => Code == op.Code && Data == op.Data;
        public static bool operator ==(Operand x, Operand y) => x.Equals(y);
        public static bool operator !=(Operand x, Operand y) => !(x == y);

    }
}
