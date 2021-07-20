﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Units;
using CafeLib.BsvSharp.UnitTests.Extensions;
using CafeLib.Core.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Scripts
{
    public class KzSigHashTests
    {
        /// <summary>
        /// Test Vector
        /// </summary>
        private class TestVector
        {
            public readonly string RawTx;
            public readonly string RawScript;
            public readonly int Index;
            public readonly SignatureHashType SigHashType;
            public readonly string SigHashRegHex;
            public readonly string SigHashOldHex;
            public readonly string SigHashRepHex;

            public TestVector(string rawTx, string script, int inputIndex, int hashType, string sigHashReg, string sigHashNoFork, string sigHashFork)
            {
                RawTx = rawTx;
                RawScript = script;
                Index = inputIndex;
                SigHashType = new SignatureHashType((uint)hashType);
                SigHashRegHex = sigHashReg;
                SigHashOldHex = sigHashNoFork;
                SigHashRepHex = sigHashFork;
            }
        }

        private readonly List<TestVector> _tvsForkId;
        private readonly List<TestVector> _tvsOther;

        public KzSigHashTests()
        {
            var tvs = new List<TestVector>();
            var json = JArray.Parse(File.ReadAllText(@"..\..\..\data\sighash.json"));
            json.Children<JToken>().Where(c => c.Count() >= 6)
                .ForEach(x =>
                {
                    var rawTx = x[0]?.Value<string>();
                    var script = x[1]?.Value<string>();
                    var inputIndex = x[2]?.Value<int>() ?? -1;
                    var hashType = x[3]?.Value<int>() ?? default;
                    var sigHashReg = x[4]?.Value<string>();
                    var sigHashNoFork = x[5]?.Value<string>();
                    var sigHashFork = string.Empty; // r[6].Value<string>();
                    tvs.Add(new TestVector(rawTx, script, inputIndex, hashType, sigHashReg, sigHashNoFork, sigHashFork));
                });

            (_tvsForkId, _tvsOther) = tvs.Partition(tv => tv.SigHashType.HasForkId);
        }

        static void SigHash(IEnumerable<TestVector> tvs)
        {
            foreach (var tv in tvs)
            {
                var tx = Transaction.ParseHex(tv.RawTx);
                var (scriptCodeOk, scriptCode) = Script.ParseHex(tv.RawScript, withoutLength: true);
                Assert.True(tx != null && scriptCodeOk);

                var shreg = TransactionSignatureChecker.ComputeSignatureHash(scriptCode, tx, tv.Index, tv.SigHashType, Amount.Zero).ToString();
                Assert.Equal(tv.SigHashRegHex, shreg);

                var shold = TransactionSignatureChecker.ComputeSignatureHash(scriptCode, tx, tv.Index, tv.SigHashType, Amount.Zero, 0).ToString();
                Assert.Equal(tv.SigHashOldHex, shold);
            }
        }

        [Fact]
        public void SigHashForkId()
        {
            SigHash(_tvsForkId);
        }

        [Fact]
        public void SigHashOther()
        {
            SigHash(_tvsOther);
        }
    }
}
