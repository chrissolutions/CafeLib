#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Scripting;

namespace CafeLib.Bitcoin.Shared.Builders
{
    public class ScriptBuilder
    {
        /// <summary>
        /// true if no more additions or removals from the operations will occur,
        /// but note that individual operations may still NOT be final.
        /// false by default.
        /// </summary>
        protected bool _isFinal;

        /// <summary>
        /// true if script is associated with a scriptPub.
        /// false if script is associated with a scriptSig.
        /// null if script purpose is unknown.
        /// </summary>
        protected bool? _isPub;

        /// <summary>
        /// If the script implements a known template, this will be the template type.
        /// Otherwise it will be Unknown.
        /// </summary>
        protected TemplateId _TemplateId;

        /// <summary>
        /// The sequence of operations where each operation is an opcode and optional data.
        /// To support testing and unimplemented features, an operation's IsRaw flag can be set in
        /// which case the opcode is ignored and the data is treated as unparsed script code.
        /// </summary>
        protected List<OperandBuilder> _ops = new List<OperandBuilder>();

        public List<OperandBuilder> Ops => _ops;

        /// <summary>
        /// true when no more additions, deletions or changes to existing operations will occur.
        /// </summary>
        public bool IsFinal => _isFinal && _ops.All(op => op.IsFinal);
        public bool IsPub { get => _isPub == true; set => _isPub = value ? (bool?)true : null; }
        public bool IsSig { get => _isPub == false; set => _isPub = value ? (bool?)false : null; }
        public TemplateId TemplateId => _TemplateId;
        public long Length => _ops.Sum(o => o.Length);

        public ScriptBuilder()
        {
        }

        public ScriptBuilder(byte[] script)
        {
            Set(new Script(script));
        }

        public ScriptBuilder(Script script)
        {
            Set(script);
        }

        public ScriptBuilder Clear()
        {
            _ops.Clear(); 
            return this;
        }

        public ScriptBuilder Set(Script script)
        {
            _ops.Clear(); 
            return Add(script);
        }

        public ScriptBuilder Add(Opcode opc)
        {
            _ops.Add(new Operand(opc));
            return this;
        }

        public ScriptBuilder Add(Opcode opc, ValType v)
        {
            _ops.Add(new Operand(opc, v)); 
            return this;
        }

        public ScriptBuilder Add(OperandBuilder opBuilder)
        {
            _ops.Add(opBuilder); 
            return this;
        }

        public ScriptBuilder Add(Script script)
        {
           // _ops.AddRange(script.ToBOps()); 
            return this;
        }

        public ScriptBuilder Add(string hex)
        {
            //return Add(hex.ToScript());
            return this;
        }

        public ScriptBuilder Add(byte[] raw)
        {
            //_ops.Add(new KzBOp(new ValType(raw)));
            return this;
        }

        /// <summary>
        /// Push a zero as a non-final placeholder.
        /// </summary>
        /// <returns></returns>
        //public ScriptBuilder Push() => Add(new KzBOp { IsFinal = false, IsRaw = false, Op = new KzOp(KzOpcode.OP_0) });

        public ScriptBuilder Push(ReadOnlySpan<byte> data)
        {
            _ops.Add(Operand.Push(data)); 
            return this;
        }

        public ScriptBuilder Push(long v)
        {
            _ops.Add(Operand.Push(v));
            return this;
        }

        public Script ToScript() => new Script(ToBytes());

        public byte[] ToBytes()
        {
            var bytes = new byte[Length];
            var span = (ByteSpan)bytes;
            foreach (var op in _ops) 
            {
                op.TryCopyTo(ref span);
            }
            return bytes;
        }

        //public string ToHex() => ToBytes().ToHex();

        public override string ToString()
        {
            return string.Join(' ', _ops.Select(o => o.ToVerboseString()));
        }

        public string ToTemplateString()
        {
            var sb = new StringBuilder();
            foreach (var bop in _ops) 
            {
                var op = bop.Operand;
                var len = op.Data.Length;
                sb.Append(len == 0 ? $"{op.CodeName} " : $"[{op.Data.Length}] ");
            }
            if (sb.Length > 0)
                sb.Length--;
            return sb.ToString();
        }

        /// <summary>
        /// Converts hex and ascii strings to a specific byte count, if len has a value and disagrees it is an error.
        /// Converts integer values to little endian bytes where the most significant bit is set if negative.
        /// For integer values, if len has a value, the result is expanded if necessary. If len is too small it is an error.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static byte[] ParseCompactValueToBytes(string s, uint? len = null) => ParseLiteralValueToBytes(s, len).bytes;

        /// <summary>
        /// Parses signed decimals, hexadecimal strings prefixed with 0x, and ascii strings enclosed in single quotes.
        /// Each format is converted to a byte array.
        /// Converts hex and ascii strings to a specific byte count, if len has a value and disagrees it is an error.
        /// Converts integer values to little endian bytes where the most significant bit is set if negative.
        /// For integer values, if len has a value, the result is expanded if necessary. If len is too small it is an error.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <returns>Tuple of the parsed byte[] data and a boolean true if the literal was specified in hexadecimal.
        /// Returns null for bytes if can't be parsed as a literal.</returns>
        private static (byte[] bytes, bool isHex) ParseLiteralValueToBytes(string s, uint? len = null)
        {
            var bytes = (byte[])null;
            var isHex = false;

            if (s.StartsWith("'") && s.EndsWith("'"))
            {
                s = s.Substring(1, s.Length - 2);
                if (s.Contains("'"))
                    throw new InvalidOperationException();
                bytes = System.Text.Encoding.ASCII.GetBytes(s);
            } 
            else if (s.StartsWith("0x"))
            {
                isHex = true;
                bytes = Encoders.Hex.Decode(s.Substring(2));
            } 
            else if (long.TryParse(s, out var v))
            {
                bytes = ScriptNum.Serialize(v);
            }

            if (len.HasValue && bytes != null && len.Value != bytes.Length)
                throw new InvalidOperationException();
            
            return (bytes, isHex);
        }

        /// <summary>
        /// Parses format used by script_tests.json file shared with C++ bitcoin-sv codebase.
        /// Primary difference is that hex literals are never treated as push data.
        /// Hex literals are also treated as unparsed bytes. e.g. multiple opcodes in a single literal.
        /// The use of "OP" before a literal is not used to create opcodes from literals.
        /// Instead, single byte hex literals are interpreted as opcodes directly.
        /// Test scripts also wish to encode invalid scripts to make sure the interpreter will
        /// catch the errors.
        /// </summary>
        /// <param name="testScript"></param>
        /// <returns></returns>
        public static ScriptBuilder ParseTestScript(string testScript)
        {
            var sb = new ScriptBuilder();
            var ps = testScript.Split(' ', StringSplitOptions.RemoveEmptyEntries).AsSpan();
            while (ps.Length > 0) {
                var arg = 0;
                var (bytes, isHex) = ParseLiteralValueToBytes(ps[arg]);
                if (bytes != null) {
                    if (isHex)
                        // Hex literals are treated as raw, unparsed bytes added to the script.
                        sb.Add(bytes);
                    else
                        sb.Push(bytes);
                } else {
                    var data = (byte[])null;
                    if (!Enum.TryParse<Opcode>("OP_" + ps[arg], out Opcode opcode))
                        throw new InvalidOperationException();
                    if (opcode > Opcode.OP_0 && opcode < Opcode.OP_PUSHDATA1) {
                        // add next single byte value to op.
                        arg++;
                        data = ParseCompactValueToBytes(ps[arg]);
                        if (data == null) {
                            // Put this arg back. Treat missing data as zero length.
                            data = new byte[0];
                            arg--;
                        }
                    } else if (opcode >= Opcode.OP_PUSHDATA1 && opcode <= Opcode.OP_PUSHDATA4) {
                        // add next one, two, or four byte value as length of following data value to op.
                        arg++;
                        var lengthBytes = ParseCompactValueToBytes(ps[arg]);
                        var len = 0u;
                        if (!BitConverter.IsLittleEndian)
                            throw new NotSupportedException();
                        if (opcode == Opcode.OP_PUSHDATA1) {
                            // add next one byte value as length of following data value to op.
                            if (lengthBytes.Length != 1)
                                throw new InvalidOperationException();
                            len = lengthBytes[0];
                        } else if (opcode == Opcode.OP_PUSHDATA2) {
                            // add next two byte value as length of following data value to op.
                            if (lengthBytes.Length != 2)
                                throw new InvalidOperationException();
                            len = BitConverter.ToUInt16(lengthBytes);
                        } else if (opcode == Opcode.OP_PUSHDATA4) {
                            // add next four byte value as length of following data value to op.
                            if (lengthBytes.Length != 4)
                                throw new InvalidOperationException();
                            len = BitConverter.ToUInt32(lengthBytes);
                        }
                        if (len > 0) {
                            arg++;
                            data = arg < ps.Length ? ParseCompactValueToBytes(ps[arg], len) : new byte[0];
                        }
                    }
                    if (data == null)
                        sb.Add(opcode);
                    else
                        sb.Add(opcode, new ValType(data));
                }
                ps = ps.Slice(Math.Min(arg + 1, ps.Length));
            }
            return sb;
        }

                    //if (!isOp && ps[arg] == "OP") {
                    //    arg++;
                    //    var opcodeBytes = ParseCompactValueToBytes(ps[arg]);
                    //    if (opcodeBytes == null || opcodeBytes.Length > 1)
                    //        throw new InvalidOperationException();
                    //    op = (KzOpcode)opcodeBytes[0];
                    //}
        public static ScriptBuilder ParseCompact(string compactScript)
        {
            var sb = new ScriptBuilder();
            var ps = compactScript.Split(' ', StringSplitOptions.RemoveEmptyEntries).AsSpan();
            while (ps.Length > 0) {
                var s = ps[0];
                var bytes = ParseCompactValueToBytes(s);
                if (bytes != null) {
                    sb.Push(bytes);
                    ps = ps.Slice(1);
                } else if (Enum.TryParse<Opcode>("OP_" + s, out Opcode op)) {
                    var args = 1;
                    var data = (byte[])null;
                    if (op > Opcode.OP_0 && op < Opcode.OP_PUSHDATA1) {
                        // add next single byte value to op.
                        args = 2;
                        data = ParseCompactValueToBytes(ps[1]);
                        if (data.Length >= (int)Opcode.OP_PUSHDATA1)
                            throw new InvalidOperationException();
                    } else if (op >= Opcode.OP_PUSHDATA1 && op <= Opcode.OP_PUSHDATA4) {
                        // add next one, two, or four byte value as length of following data value to op.
                        args = 2;
                        var lengthBytes = ParseCompactValueToBytes(ps[1]);
                        var len = 0u;
                        if (!BitConverter.IsLittleEndian)
                            throw new NotSupportedException();
                        if (op == Opcode.OP_PUSHDATA1) {
                            // add next one byte value as length of following data value to op.
                            if (lengthBytes.Length != 1)
                                throw new InvalidOperationException();
                            len = lengthBytes[0];
                        } else if (op == Opcode.OP_PUSHDATA2) {
                            // add next two byte value as length of following data value to op.
                            if (lengthBytes.Length != 2)
                                throw new InvalidOperationException();
                            len = BitConverter.ToUInt16(lengthBytes);
                        } else if (op == Opcode.OP_PUSHDATA4) {
                            // add next four byte value as length of following data value to op.
                            if (lengthBytes.Length != 4)
                                throw new InvalidOperationException();
                            len = BitConverter.ToUInt32(lengthBytes);
                        }
                        if (len > 0) {
                            args = 3;
                            data = ParseCompactValueToBytes(ps[2], len);
                        }
                    }
                    if (data == null)
                        sb.Add(op);
                    else
                        sb.Add(op, new ValType(data));
                    ps = ps.Slice(args);
                } else
                    throw new InvalidOperationException();
            }
            return sb;
        }

        public static implicit operator Script(ScriptBuilder sb) => sb.ToScript();

        public static implicit operator ScriptBuilder(Script v)
        {
            throw new NotImplementedException();
        }
    }
}
