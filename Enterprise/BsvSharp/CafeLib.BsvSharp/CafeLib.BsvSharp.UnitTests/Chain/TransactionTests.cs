#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Scripting;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Chain
{
    public class TransactionTests
    {
        private const string DataFolder = @"..\..\..\data";
        private static IEnumerable<string> GetValidTransaction()
        {
            var values = new List<string>();
            var jarray = JArray.Parse(File.ReadAllText(Path.Combine(DataFolder, "tx_valid.json")));

            foreach (var json in jarray)
            {
                var jtoken = FindValueInArray(json);
                if (jtoken.Type == JTokenType.String && jtoken.Parent.Count >= 3)
                {
                    var prevTxHash = jtoken.Parent[0].Value<string>();
                    var prevTxIndex = jtoken.Parent[1].Value<string>();
                    var prevScript = jtoken.Parent[2].Value<string>();
                    var serializedTx = jtoken.Parent.Parent.Parent[1].Value<string>();
                    var verifyFlag = jtoken.Parent.Parent.Parent[2].Value<string>();
                    values.Add(string.Join('|', prevTxHash, prevTxIndex, prevScript, serializedTx, verifyFlag));
                }
            }

            return values;

            static JToken FindValueInArray(JToken token)
            {
                if (!(token is JArray items)) return string.Empty;

                foreach (var json in items)
                {
                    return json.Type == JTokenType.Array ? FindValueInArray((JArray)json) : json;
                }

                return string.Empty;
            }
        }


        private TransactionBuilder BuildCreditingTransaction(Script scriptPubKey, long nValue)
        {
            var tx = new TransactionBuilder
            {
                LockTime = 0,
                Version = 1,
            };

            tx.AddIn(new OutPoint(), new ScriptBuilder().Push(0).Push(0));
            tx.AddOut(scriptPubKey, nValue);
            return tx;
        }

        TransactionBuilder BuildSpendingTransaction(Script scriptSig, TransactionBuilder txCredit)
        {
            var tx = new TransactionBuilder
            {
                LockTime = 0,
                Version = 1,
            };

            tx.AddIn(new OutPoint(), new ScriptBuilder().Push(0).Push(0));
            //tx.AddOut(scriptPubKey, nValue);
            return tx;
        }

        [Fact]
        public void Basic()
        {
            var lines = GetValidTransaction();
            var t = lines.Where(x => !x.Contains("P2SH"));
            Console.WriteLine(lines);
        }
    }
}