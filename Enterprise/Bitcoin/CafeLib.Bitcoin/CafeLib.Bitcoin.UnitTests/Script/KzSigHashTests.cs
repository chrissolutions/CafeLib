﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Script;
using CafeLib.Bitcoin.Utility;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Script
{
    public class KzSigHashTests
    {
        /// <summary>
        /// Test Vector
        /// </summary>
        class TestVector
        {
            public string rawTx;
            public string rawScript;
            public int nIn;
            public KzSigHashType sigHashType;
            public string sigHashRegHex;
            public string sigHashOldHex;
            public string sigHashRepHex;

            public TestVector(string rawTx, string script, int inputIndex, int hashType, string sigHashReg, string sigHashNoFork, string sigHashFork)
            {
                this.rawTx = rawTx;
                this.rawScript = script;
                this.nIn = inputIndex;
                this.sigHashType = new KzSigHashType((uint)hashType);
                this.sigHashRegHex = sigHashReg;
                this.sigHashOldHex = sigHashNoFork;
                this.sigHashRepHex = sigHashFork;
            }
        }

        List<TestVector> tvsForkId;
        List<TestVector> tvsOther;

        public KzSigHashTests()
        {
            var tvs = new List<TestVector>();
            var json = JArray.Parse(File.ReadAllText(@"..\..\..\data\sighash.json"));
            foreach (var r in json.Children<JToken>().Where(c => c.Count() >= 6)) {
                var rawTx = r[0].Value<string>();
                var script = r[1].Value<string>();
                var inputIndex = r[2].Value<int>();
                var hashType = r[3].Value<int>();
                var sigHashReg = r[4].Value<string>();
                var sigHashNoFork = r[5].Value<string>();
                var sigHashFork = string.Empty; // r[6].Value<string>();
                tvs.Add(new TestVector(rawTx, script, inputIndex, hashType, sigHashReg, sigHashNoFork, sigHashFork));
            }

            (tvsForkId, tvsOther) = tvs.Partition(tv => tv.sigHashType.hasForkId);
        }

        void SigHash(List<TestVector> tvs)
        {
            var i = 0;
            foreach (var tv in tvs) {
                i++;
                var tx = KzTransaction.ParseHex(tv.rawTx);
                var (scriptCodeOk, scriptCode) = KzScript.ParseHex(tv.rawScript, withoutLength: true);
                Assert.True(tx != null && scriptCodeOk);

                var shreg = KzScriptInterpreter.ComputeSignatureHash(scriptCode, tx, tv.nIn, tv.sigHashType, KzAmount.Zero).ToString();
                Assert.Equal(tv.sigHashRegHex, shreg);

                var shold = KzScriptInterpreter.ComputeSignatureHash(scriptCode, tx, tv.nIn, tv.sigHashType, KzAmount.Zero, 0).ToString();
                Assert.Equal(tv.sigHashOldHex, shold);
            }
        }

        [Fact]
        public void SigHashForkId() { SigHash(tvsForkId); }

        [Fact]
        public void SigHashOther() { SigHash(tvsOther); }

    }
}
