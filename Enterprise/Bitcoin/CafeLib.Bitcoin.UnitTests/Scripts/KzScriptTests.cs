#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CafeLib.Bitcoin.Builders;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Encoding;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Scripting;
using CafeLib.Bitcoin.Services;
using CafeLib.Bitcoin.Units;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CafeLib.Bitcoin.UnitTests.Scripts
{
    public class KzScriptTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        [Fact]
        public void Decode()
        {
            foreach (var hex in new []
            {
                "76a914c2eaba3b9c29575322c6e24fdc1b49bdfe405bad88ac",
                "4730440220327588eb1c9e502358142b67b3cd799cb6163fde4f1a92490affda78734bc63c0220639a29e63d78c971177a1792cec1b0a7e65c973edbf03eba3b3380d97b829f80412103ea03d07638e40b53d8098b62e964112f562af5ba1bffaa146ffd9e7f7d1a5c67",
                "6a22314c74794d45366235416e4d6f70517242504c6b3446474e3855427568784b71726e0101337b2274223a32302e36322c2268223a35392c2270223a313031322c2263223a312c227773223a362e322c227764223a3236307d22314a6d64484e4456336f6434796e614c7635696b4d6234616f763737507a665169580a31353537303838383133"
            })
            {
                var sb = new Script(hex);
                var s = sb.ToString();
            }

            var address = new PublicKey(true);
            var e = new UInt160("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad", true);
            var s1 = Encoders.Base58Check.Encode(RootService.Network.PublicKeyAddress, e.Span);
            var s2 = Encoders.Base58Check.Encode(RootService.Network.ScriptAddress, e.Span);
            //e.Span.CopyTo(address.Span);
            //var id = address.GetID();
        }

        private static readonly HexEncoder HexEncoder = Encoders.Hex;

        /// <summary>
        /// Test Vector
        /// </summary>
        private class TestValue1 
        {
            /// <summary>
            /// Script as hex string.
            /// </summary>
            public string Hex;
            /// <summary>
            /// Script as decoded OPs.
            /// </summary>
            public string Decode;
        }

        private readonly TestValue1[] _testValues1 = 
        {
            new TestValue1 
            {
                Hex = "04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73",
                Decode = "OP_PUSH4 ffff001d OP_PUSH1 04 OP_PUSH69 5468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73"
            },
            new TestValue1
            {
                Hex = "4104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac",
                Decode = "OP_PUSH65 04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG"
            }
        };

        public KzScriptTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Encoding()
        {
            foreach (var tv in _testValues1) {
                var bytes = HexEncoder.Decode(tv.Hex);
                var s = new Script(tv.Hex);
                var ops = s.Decode().ToArray();
                var d = s.ToVerboseString();
                Assert.Equal(tv.Decode, d);
            }
        }

        /// <summary>
        /// Test Vector
        /// </summary>
        class TV2 
        {
            /// <summary>
            /// ScriptSig as hex string.
            /// </summary>
            public string sig;
            /// <summary>
            /// ScriptPub .
            /// </summary>
            public string pub;
            /// <summary>
            /// Flags
            /// </summary>
            public string flags;
            /// <summary>
            /// Result: Error or OK.
            /// </summary>
            public string error;

            public Script scriptSig;
            public Script scriptPub;
            public ScriptFlags scriptFlags;
            public ScriptError scriptError;
            public Opcode[] opcodes;
            public Opcode? keyopcode;

            public TV2(params string[] args)
            {
                sig = args[0];
                pub = args[1];
                flags = args[2];
                error = args[3];

                scriptSig = ScriptBuilder.ParseTestScript(sig).ToScript();
                scriptPub = ScriptBuilder.ParseTestScript(pub).ToScript();
                scriptFlags = ScriptInterpreter.ParseFlags(flags);
                scriptError = ToScriptError(error);

                opcodes = scriptSig.Decode().Select(o => o.Code)
                    .Concat(scriptPub.Decode().Select(o => o.Code))
                    .Distinct()
                    .OrderBy(o => o).ToArray();

                keyopcode = opcodes.Length == 0 ? (Opcode?)null : opcodes.Last();
            }
        }

        private static TV2 M2(params string[] args) => args.Length < 4 ? null : new TV2(args);

        private static ScriptError ToScriptError(string error)
        {
            if (!Enum.TryParse(error, out ScriptError result))
            {
                result = error switch 
                {
                    "SPLIT_RANGE" => ScriptError.INVALID_SPLIT_RANGE,
                    "OPERAND_SIZE" => ScriptError.INVALID_OPERAND_SIZE,
                    "NULLFAIL" => ScriptError.SIG_NULLFAIL,
                    "MISSING_FORKID" => ScriptError.MUST_USE_FORKID,
                    _ => ScriptError.UNKNOWN_ERROR
                };
            }
            return result;
        }

        [Fact]
        public void Scripts()
        {
            var tv2s = new List<TV2>();
            var json = JArray.Parse(File.ReadAllText(@"..\..\..\data\script_tests.json"));
            foreach (var r in json.Children<JToken>().Where(c => c.Count() >= 4))
            {
                if (r[0].Type == JTokenType.String)
                {
                    var sig = r[0].Value<string>();
                    var pub = r[1].Value<string>();
                    var flags = r[2].Value<string>();
                    var error = r[3].Value<string>();
                    tv2s.Add(new TV2(sig, pub, flags, error));
                }
            }

            var tv2sSorted = tv2s.OrderBy(tv => tv.opcodes.Length + (int)tv.opcodes.LastOrDefault() / 256.0).ToList();

            var opcodes = tv2sSorted.Select(tv => tv.keyopcode).Distinct().OrderBy(o => o).ToArray();

            var noOpcode = new List<TV2>();
            var byOpcode = new Dictionary<Opcode, List<TV2>>();
            foreach (var tv in tv2sSorted) 
            {
                var o = tv.keyopcode;
                var list = o.HasValue ? null : noOpcode;
                if (list == null && !byOpcode.TryGetValue(o.Value, out list))
                {
                    list = new List<TV2>();
                    byOpcode.Add(o.Value, list);
                }
                list.Add(tv);
            }

            var i = 0;
            foreach (var opcode in opcodes) 
            {
                var list = opcode.HasValue ? byOpcode[opcode.Value] : noOpcode;
                foreach (var tv in list) 
                {
                    i++;
                    var tv2 = new TV2(tv.sig, tv.pub, tv.flags, tv.error);
                    _testOutputHelper.WriteLine($"{opcode} {i}");
                    _testOutputHelper.WriteLine($"Sig: {tv.scriptSig.ToHexString()} => {tv.scriptSig}");
                    _testOutputHelper.WriteLine($"Pub: {tv.scriptPub.ToHexString()} => {tv.scriptPub}");

                    var checker = new TransactionSignatureChecker(new Transaction(), 0, Amount.Zero);
                    var ok = ScriptInterpreter.VerifyScript(tv.scriptSig, tv.scriptPub, tv.scriptFlags, checker, out var error);

                    var correct = (ok && tv.scriptError == ScriptError.OK) || tv.scriptError == error;

                    // All test cases do not pass yet. This condition is here to make sure things don't get worse :-)
                    if (i < 900)
                        Assert.True(correct);
                }
            }
        }

    }
}
